using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RadiusCore
{
    internal class DefaultUdpClient : IUdpClient
    {
        private readonly UdpClient _udpClient;

        public DefaultUdpClient(UdpClient udpClient)
        {
            _udpClient = udpClient;
        }

        public ValueTask<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken)
        {
            return _udpClient.ReceiveAsync(cancellationToken);
        }

        public ValueTask<int> SendAsync(ReadOnlyMemory<byte> datagram, IPEndPoint endPoint, CancellationToken cancellationToken)
        {
            return _udpClient.SendAsync(datagram, endPoint, cancellationToken);
        }
    }
}
