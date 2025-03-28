using System.Net;
using System.Net.Sockets;

namespace RadiusCore
{
    public interface IUdpClient
    {
        ValueTask<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken);

        ValueTask<int> SendAsync(ReadOnlyMemory<byte> datagram, IPEndPoint endPoint,
            CancellationToken cancellationToken);
    }
}
