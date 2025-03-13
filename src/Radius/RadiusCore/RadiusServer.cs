using System.Diagnostics;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using RadiusCore.Packet;
using RadiusCore.RadiusConstants;

namespace RadiusCore;

/// <summary>
/// Create a new server on endpoint with packet handler repository
/// </summary>
/// <param name="udpClientFactory"></param>
/// <param name="localEndpoint"></param>
/// <param name="radiusPacketParser"></param>
/// <param name="serverType"></param>
/// <param name="packetHandlerRepository"></param>
/// <param name="logger"></param>
public sealed class RadiusServer(IUdpClientFactory udpClientFactory, IPEndPoint localEndpoint, IRadiusPacketParser radiusPacketParser, RadiusServerType serverType, IPacketHandlerRepository packetHandlerRepository, ILogger<RadiusServer> logger) : IDisposable
{
    private IUdpClient? _server;
    private int _concurrentHandlerCount;
    private readonly ILogger _logger = logger;

    public bool Running
    {
        get;
        private set;
    }

    /// <summary>
    /// Add packet handler for remote endpoint
    /// </summary>
    /// <param name="remoteAddress"></param>
    /// <param name="sharedSecret"></param>
    /// <param name="packetHandler"></param>
    [Obsolete("Use methods on IPacketHandlerRepository implementation instead")]
    public void AddPacketHandler(IPAddress remoteAddress, string sharedSecret, IPacketHandler packetHandler)
    {
        _logger.LogInformation($"Adding packet handler of type {packetHandler.GetType()} for remote IP {remoteAddress} to {serverType}Server");
        packetHandlerRepository.AddPacketHandler(remoteAddress, packetHandler, sharedSecret);
    }


    /// <summary>
    /// Add packet handler for network range
    /// </summary>
    /// <param name="network"></param>
    /// <param name="sharedSecret"></param>
    /// <param name="packetHandler"></param>
    [Obsolete("Use methods on IPacketHandlerRepository implementation instead")]
    public void AddPacketHandler(IPNetwork network, string sharedSecret, IPacketHandler packetHandler)
    {
        packetHandlerRepository.Add(network, packetHandler, sharedSecret);
    }


    /// <summary>
    /// Start listening for requests
    /// </summary>
    public async Task Start()
    {
        if (!Running)
        {
            _server = udpClientFactory.CreateClient(localEndpoint);
            Running = true;
            _logger.LogInformation($"Starting Radius server on {localEndpoint}");
            await StartReceiveLoopAsync();
            _logger.LogInformation("Server started");
        }
        else
        {
            _logger.LogWarning("Server already started");
        }
    }


    /// <summary>
    /// Stop listening
    /// </summary>
    public void Stop()
    {
        if (Running)
        {
            _logger.LogInformation("Stopping server");
            Running = false;
            _server?.Dispose();
            _logger.LogInformation("Stopped");
        }
        else
        {
            _logger.LogWarning("Server already stopped");
        }
    }


    /// <summary>
    /// Start the loop used for receiving packets
    /// </summary>
    /// <returns></returns>
    private async Task StartReceiveLoopAsync()
    {
        while (Running)
        {
            try
            {
                var response = await _server!.ReceiveAsync();
                await Task.Factory.StartNew(async () => await HandlePacket(response.RemoteEndPoint, response.Buffer), TaskCreationOptions.LongRunning);
            }
            catch (ObjectDisposedException) { } // This is thrown when udpclient is disposed, can be safely ignored
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Something went wrong receiving packet");
            }
        }
    }


    /// <summary>
    /// Used to handle the packets asynchronously
    /// </summary>
    /// <param name="remoteEndpoint"></param>
    /// <param name="packetBytes"></param>
    private async Task HandlePacket(IPEndPoint remoteEndpoint, byte[] packetBytes)
    {
        try
        {
            _logger.LogDebug($"Received packet from {remoteEndpoint}, Concurrent handlers count: {Interlocked.Increment(ref _concurrentHandlerCount)}");

            if (packetHandlerRepository.TryGetHandler(remoteEndpoint.Address, out var handler))
            {
                var responsePacket = GetResponsePacket(handler.packetHandler, handler.sharedSecret, packetBytes, remoteEndpoint);
                if (responsePacket != null)
                {
                    await SendResponsePacket(responsePacket, remoteEndpoint);
                }
            }
            else
            {
                _logger.LogError($"No packet handler found for remote ip {remoteEndpoint}");
                var packet = radiusPacketParser.Parse(packetBytes, "wut"u8.ToArray());
                DumpPacket(packet);
            }
        }
        catch (Exception ex) when (ex is ArgumentException || ex is OverflowException)
        {
            _logger.LogWarning($"Ignoring malformed(?) packet received from {remoteEndpoint}", ex);
            _logger.LogDebug(packetBytes.ToHexString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to receive packet from {remoteEndpoint}");
            _logger.LogDebug(packetBytes.ToHexString());
        }
        finally
        {
            Interlocked.Decrement(ref _concurrentHandlerCount);
        }
    }


    /// <summary>
    /// Parses a packet and gets a response packet from the handler
    /// </summary>
    /// <param name="packetHandler"></param>
    /// <param name="sharedSecret"></param>
    /// <param name="packetBytes"></param>
    /// <param name="remoteEndpoint"></param>
    /// <returns></returns>
    internal RadiusPacket GetResponsePacket(IPacketHandler packetHandler, string sharedSecret, byte[] packetBytes, IPEndPoint remoteEndpoint)
    {
        var requestPacket = radiusPacketParser.Parse(packetBytes, Encoding.UTF8.GetBytes(sharedSecret));
        _logger.LogInformation($"Received {requestPacket.Code} from {remoteEndpoint} Id={requestPacket.Identifier}");

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            DumpPacket(requestPacket);
        }
        _logger.LogDebug(packetBytes.ToHexString());

        // Handle status server requests in server outside packet handler
        if (requestPacket.Code == PacketCode.StatusServer)
        {
            var responseCode = serverType == RadiusServerType.Authentication ? PacketCode.AccessAccept : PacketCode.AccountingResponse;
            _logger.LogDebug($"Sending {responseCode} for StatusServer request from {remoteEndpoint}");
            return requestPacket.CreateResponsePacket(responseCode);
        }

        _logger.LogDebug($"Handling packet for remote ip {remoteEndpoint.Address} with {packetHandler.GetType()}");

        var sw = Stopwatch.StartNew();
        var responsePacket = packetHandler.HandlePacket(requestPacket);
        sw.Stop();
        _logger.LogDebug($"{remoteEndpoint} Id={responsePacket.Identifier}, Received {responsePacket.Code} from handler in {sw.ElapsedMilliseconds}ms");
        if (sw.ElapsedMilliseconds >= 5000)
        {
            _logger.LogWarning($"Slow response for Id {responsePacket.Identifier}, check logs");
        }

        if (requestPacket.Attributes.ContainsKey("Proxy-State"))
        {
            responsePacket.Attributes.Add("Proxy-State", requestPacket.Attributes.SingleOrDefault(o => o.Key == "Proxy-State").Value);
        }

        return responsePacket;
    }


    /// <summary>
    /// Sends a packet
    /// </summary>
    /// <param name="responsePacket"></param>
    /// <param name="remoteEndpoint"></param>
    private async Task SendResponsePacket(RadiusPacket responsePacket, IPEndPoint remoteEndpoint)
    {
        var responseBytes = radiusPacketParser.GetBytes(responsePacket);
        await _server!.SendAsync(responseBytes, responseBytes.Length, remoteEndpoint);   // todo thread safety... although this implementation will be implicitly thread safeish...
        _logger.LogInformation($"{responsePacket.Code} sent to {remoteEndpoint} Id={responsePacket.Identifier}");
    }


    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        _server?.Dispose();
    }


    /// <summary>
    /// Dump the packet attributes to the log
    /// </summary>
    /// <param name="packet"></param>
    private void DumpPacket(RadiusPacket packet)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Packet dump for {packet.Identifier}:");
        foreach (var attribute in packet.Attributes)
        {
            if (attribute.Key == "User-Password")
            {
                sb.AppendLine($"{attribute.Key} length : {attribute.Value.First().ToString()!.Length}");
            }
            else
            {
                attribute.Value.ForEach(o => sb.AppendLine($"{attribute.Key} : {o} [{o.GetType()}]"));
            }
        }

        _logger.LogDebug(sb.ToString());
    }
}