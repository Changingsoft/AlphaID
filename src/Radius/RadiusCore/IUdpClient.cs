using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RadiusCore
{
    public interface IUdpClient
    {
        ValueTask<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken);

        ValueTask<int> SendAsync(ReadOnlyMemory<byte> datagram, IPEndPoint endPoint,
            CancellationToken cancellationToken);
    }
}
