using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RadiusCore.Packet;
using RadiusCore.RadiusConstants;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RadiusCore;

/// <summary>
/// RADIUS server.
/// </summary>
/// <param name="udpClientFactory">Udp client factory for create a UDP client.</param>
/// <param name="radiusPacketParser"></param>
/// <param name="packetHandlerRepository"></param>
/// <param name="options">Options for RADIUS server.</param>
/// <param name="logger">Logger for RADIUS server.</param>
public class RadiusServer(
    IUdpClientFactory udpClientFactory,
    IRadiusPacketParser radiusPacketParser,
    IPacketHandlerRepository packetHandlerRepository,
    IOptions<RadiusServerOptions> options,
    ILogger<RadiusServer>? logger) : IHostedService, IDisposable
{
    private IUdpClient? _udpClient;
    private int _concurrentHandlerCount;
    private Task? _receiveLoopTask;
    private CancellationTokenSource? _stoppingCts;
    private readonly RadiusServerType _radiusServerType = RadiusServerType.Authentication;

    /// <summary>
    /// Add packet handler for remote endpoint
    /// </summary>
    /// <param name="remoteAddress"></param>
    /// <param name="sharedSecret"></param>
    /// <param name="packetHandler"></param>
    [Obsolete("Use methods on IPacketHandlerRepository implementation instead")]
    public void AddPacketHandler(IPAddress remoteAddress, string sharedSecret, IPacketHandler packetHandler)
    {
        logger?.LogInformation("Adding packet handler of type {packetHandler} for remote IP {remoteAddress} to {serverType}Server", packetHandler.GetType(), remoteAddress, _radiusServerType);
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
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var localEndpoint = new IPEndPoint(IPAddress.Any, options.Value.AuthenticationServerPort);

        logger?.LogInformation("Starting Radius server on {localEndpoint}", localEndpoint);

        _udpClient = udpClientFactory.CreateClient(localEndpoint);

        _receiveLoopTask = ReceiveLoopTask(_stoppingCts.Token);

        if (_receiveLoopTask.IsCompleted)
            return _receiveLoopTask;

        return Task.CompletedTask;
    }


    /// <summary>
    /// Stop listening
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_receiveLoopTask == null)
            return;

        try
        {
            await _stoppingCts!.CancelAsync();
        }
        finally
        {
            await _receiveLoopTask.WaitAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
        }
    }


    /// <summary>
    /// Start the loop used for receiving packets
    /// </summary>
    /// <returns></returns>
    private async Task ReceiveLoopTask(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                UdpReceiveResult result = await _udpClient!.ReceiveAsync(cancellationToken);
                //读取报文并创建处理上下文
                RadiusPacketStruct radiusPacketStruct =
                    RadiusPacketStructExtensions.FromByteArray(result.Buffer, out var attributes);


                await HandlePacket(result.RemoteEndPoint, result.Buffer);
            }
            catch (OperationCanceledException)
            {
                logger?.LogError("服务已被取消。");
            }
            catch (Exception ex)
            {
                logger?.LogCritical(ex, "Something went wrong receiving packet");
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
            logger?.LogDebug("Received packet from {remoteEndpoint}, Concurrent handlers count: {concurrentHandlerCount)}", remoteEndpoint, Interlocked.Increment(ref _concurrentHandlerCount));

            if (packetHandlerRepository.TryGetHandler(remoteEndpoint.Address, out var handler))
            {
                var responsePacket = GetResponsePacket(handler.packetHandler, handler.sharedSecret, packetBytes, remoteEndpoint);
                await SendResponsePacket(responsePacket, remoteEndpoint);
            }
            else
            {
                logger?.LogError("No packet handler found for remote ip {remoteEndpoint}", remoteEndpoint);
                var packet = radiusPacketParser.Parse(packetBytes, "wut"u8.ToArray());
                DumpPacket(packet);
            }
        }
        catch (Exception ex) when (ex is ArgumentException || ex is OverflowException)
        {
            logger?.LogWarning("Ignoring malformed(?) packet received from {remoteEndpoint}", ex);
            logger?.LogDebug("Packet bytes: {PacketBytes}", packetBytes.ToHexString());
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to receive packet from {remoteEndpoint}", remoteEndpoint);
            logger?.LogDebug("Packet bytes: {PacketBytes}", packetBytes.ToHexString());
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
        logger?.LogInformation("Received {requestPacket.Code} from {remoteEndpoint} Id={requestPacket.Identifier}", requestPacket.Code, remoteEndpoint, requestPacket.Identifier);

        if (logger != null && logger.IsEnabled(LogLevel.Debug))
        {
            DumpPacket(requestPacket);
        }
        logger?.LogDebug("Packet bytes: {PacketBytes}", packetBytes.ToHexString());

        // Handle status server requests in server outside packet handler
        if (requestPacket.Code == PacketCode.StatusServer)
        {
            var responseCode = _radiusServerType == RadiusServerType.Authentication ? PacketCode.AccessAccept : PacketCode.AccountingResponse;
            logger?.LogDebug("Sending {responseCode} for StatusServer request from {remoteEndpoint}", responseCode, remoteEndpoint);
            return requestPacket.CreateResponsePacket(responseCode);
        }

        logger?.LogDebug("Handling packet for remote ip {remoteEndpoint.Address} with {packetHandler.GetType()}", remoteEndpoint.Address, packetHandler.GetType());

        var sw = Stopwatch.StartNew();
        var responsePacket = packetHandler.HandlePacket(requestPacket);
        sw.Stop();
        logger?.LogDebug("{remoteEndpoint} Id={responsePacket.Identifier}, Received {responsePacket.Code} from handler in {sw.ElapsedMilliseconds}ms", remoteEndpoint, responsePacket.Identifier, responsePacket.Code, sw.ElapsedMilliseconds);
        if (sw.ElapsedMilliseconds >= 5000)
        {
            logger?.LogWarning("Slow response for Id {responsePacket.Identifier}, check logs", responsePacket.Identifier);
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
        await _udpClient!.SendAsync(responseBytes, responseBytes.Length, remoteEndpoint);   // todo thread safety... although this implementation will be implicitly thread safeish...
        logger?.LogInformation("{responsePacket.Code} sent to {remoteEndpoint} Id={responsePacket.Identifier}", responsePacket.Code, remoteEndpoint, responsePacket.Identifier);
    }


    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        _udpClient?.Dispose();
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

        logger?.LogDebug("Packet: {Packet}", sb.ToString());
    }
}