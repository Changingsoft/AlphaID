using Flexinets.Net;
using Flexinets.Radius.Core;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flexinets.Radius
{
    public sealed class RadiusServer : IDisposable
    {
        private IUdpClient _server;
        private IUdpClientFactory _udpClientFactory;
        private readonly IPEndPoint _localEndpoint;
        private readonly IRadiusPacketParser _radiusPacketParser;
        private readonly RadiusServerType _serverType;
        private Int32 _concurrentHandlerCount = 0;
        private readonly IPacketHandlerRepository _packetHandlerRepository;
        private readonly ILogger _logger;

        public Boolean Running
        {
            get;
            private set;
        }


        /// <summary>
        /// Create a new server on endpoint
        /// </summary>
        /// <param name="udpClientFactory"></param>
        /// <param name="localEndpoint"></param>
        /// <param name="radiusPacketParser"></param>
        /// <param name="serverType"></param>
        /// <param name="logger"></param>
        public RadiusServer(IUdpClientFactory udpClientFactory, IPEndPoint localEndpoint, IRadiusPacketParser radiusPacketParser, RadiusServerType serverType, ILogger<RadiusServer> logger) : this
            (udpClientFactory, localEndpoint, radiusPacketParser, serverType, new PacketHandlerRepository(), logger)
        {
        }


        /// <summary>
        /// Create a new server on endpoint with packet handler repository
        /// </summary>
        /// <param name="udpClientFactory"></param>
        /// <param name="localEndpoint"></param>
        /// <param name="radiusPacketParser"></param>
        /// <param name="serverType"></param>
        /// <param name="packetHandlerRepository"></param>
        /// <param name="logger"></param>
        public RadiusServer(IUdpClientFactory udpClientFactory, IPEndPoint localEndpoint, IRadiusPacketParser radiusPacketParser, RadiusServerType serverType, IPacketHandlerRepository packetHandlerRepository, ILogger<RadiusServer> logger)
        {
            _udpClientFactory = udpClientFactory;
            _localEndpoint = localEndpoint;
            _radiusPacketParser = radiusPacketParser;
            _serverType = serverType;
            _packetHandlerRepository = packetHandlerRepository;
            _logger = logger;
        }


        /// <summary>
        /// Add packet handler for remote endpoint
        /// </summary>
        /// <param name="remoteAddress"></param>
        /// <param name="sharedSecret"></param>
        /// <param name="packetHandler"></param>
        [Obsolete("Use methods on IPacketHandlerRepository implementation instead")]
        public void AddPacketHandler(IPAddress remoteAddress, String sharedSecret, IPacketHandler packetHandler)
        {
            _logger.LogInformation($"Adding packet handler of type {packetHandler.GetType()} for remote IP {remoteAddress} to {_serverType}Server");
            _packetHandlerRepository.AddPacketHandler(remoteAddress, packetHandler, sharedSecret);
        }


        /// <summary>
        /// Add packet handler for multiple remote endpoints
        /// </summary>
        /// <param name="remoteAddresses"></param>
        /// <param name="sharedSecret"></param>
        /// <param name="packetHandler"></param>
        [Obsolete("Use methods on IPacketHandlerRepository implementation instead")]
        public void AddPacketHandler(List<IPAddress> remoteAddresses, String sharedSecret, IPacketHandler packetHandler)
        {
            _packetHandlerRepository.AddPacketHandler(remoteAddresses, packetHandler, sharedSecret);
        }


        /// <summary>
        /// Add packet handler for network range
        /// </summary>
        /// <param name="remoteAddresses"></param>
        /// <param name="sharedSecret"></param>
        /// <param name="packetHandler"></param>
        [Obsolete("Use methods on IPacketHandlerRepository implementation instead")]
        public void AddPacketHandler(IPNetwork network, String sharedSecret, IPacketHandler packetHandler)
        {
            _packetHandlerRepository.Add(network, packetHandler, sharedSecret);
        }


        /// <summary>
        /// Start listening for requests
        /// </summary>
        public void Start()
        {
            if (!Running)
            {
                _server = _udpClientFactory.CreateClient(_localEndpoint);
                Running = true;
                _logger.LogInformation($"Starting Radius server on {_localEndpoint}");
                var receiveTask = StartReceiveLoopAsync();
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
                    var response = await _server.ReceiveAsync();
                    var task = Task.Factory.StartNew(() => HandlePacket(response.RemoteEndPoint, response.Buffer), TaskCreationOptions.LongRunning);
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
        private void HandlePacket(IPEndPoint remoteEndpoint, Byte[] packetBytes)
        {
            try
            {
                _logger.LogDebug($"Received packet from {remoteEndpoint}, Concurrent handlers count: {Interlocked.Increment(ref _concurrentHandlerCount)}");

                if (_packetHandlerRepository.TryGetHandler(remoteEndpoint.Address, out var handler))
                {
                    var responsePacket = GetResponsePacket(handler.packetHandler, handler.sharedSecret, packetBytes, remoteEndpoint);
                    if (responsePacket != null)
                    {
                        SendResponsePacket(responsePacket, remoteEndpoint);
                    }
                }
                else
                {
                    _logger.LogError($"No packet handler found for remote ip {remoteEndpoint}");
                    var packet = _radiusPacketParser.Parse(packetBytes, Encoding.UTF8.GetBytes("wut"));
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
        internal IRadiusPacket GetResponsePacket(IPacketHandler packetHandler, String sharedSecret, Byte[] packetBytes, IPEndPoint remoteEndpoint)
        {
            var requestPacket = _radiusPacketParser.Parse(packetBytes, Encoding.UTF8.GetBytes(sharedSecret));
            _logger.LogInformation($"Received {requestPacket.Code} from {remoteEndpoint} Id={requestPacket.Identifier}");

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                DumpPacket(requestPacket);
            }
            _logger.LogDebug(packetBytes.ToHexString());

            // Handle status server requests in server outside packet handler
            if (requestPacket.Code == PacketCode.StatusServer)
            {
                var responseCode = _serverType == RadiusServerType.Authentication ? PacketCode.AccessAccept : PacketCode.AccountingResponse;
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
        private void SendResponsePacket(IRadiusPacket responsePacket, IPEndPoint remoteEndpoint)
        {
            var responseBytes = _radiusPacketParser.GetBytes(responsePacket);
            _server.Send(responseBytes, responseBytes.Length, remoteEndpoint);   // todo thread safety... although this implementation will be implicitly thread safeish...
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
        private void DumpPacket(IRadiusPacket packet)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Packet dump for {packet.Identifier}:");
            foreach (var attribute in packet.Attributes)
            {
                if (attribute.Key == "User-Password")
                {
                    sb.AppendLine($"{attribute.Key} length : {attribute.Value.First().ToString().Length}");
                }
                else
                {
                    attribute.Value.ForEach(o => sb.AppendLine($"{attribute.Key} : {o} [{o.GetType()}]"));
                }
            }

            _logger.LogDebug(sb.ToString());
        }
    }
}