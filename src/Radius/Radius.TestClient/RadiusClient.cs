using RadiusCore;
using RadiusCore.Packet;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Radius.TestClient;


/// <summary>
/// Create a radius client which sends and receives responses on localEndpoint
/// </summary>
public class RadiusClient(RadiusRequestParser radiusPacketParser) : IDisposable
{
    private readonly UdpClient _udpClient = new(AddressFamily.InterNetwork);
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    /// <summary>
    /// Send a packet with specified timeout
    /// </summary>
    public async Task<RadiusPacket> SendPacketAsync(RadiusRequest packet, IPEndPoint remoteEndpoint, TimeSpan timeout)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _cancellationTokenSource.Cancel();
        _udpClient.Dispose();
    }

    public record PendingRequest(byte Identifier, IPEndPoint RemoteEndpoint);

}