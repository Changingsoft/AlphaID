using System.Net;
using System.Net.Sockets;

namespace RadiusCore.Tests
{
    internal class RemoteUdpClient : IUdpClient
    {
        private TaskCompletionSource<UdpReceiveResult> _source = new TaskCompletionSource<UdpReceiveResult>();
        private readonly MockUdpClient _mock;

        internal RemoteUdpClient(IPEndPoint localEndPoint, MockUdpClient mock)
        {
            LocalEndpoint = localEndPoint;
            _mock = mock;
        }

        public IPEndPoint LocalEndpoint { get; }

        public void Transmit(UdpReceiveResult result)
        {
            _source.SetResult(result);
        }

        public async ValueTask<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken)
        {
            var result = await _source.Task;
            _source = new TaskCompletionSource<UdpReceiveResult>();
            return result;
        }

        public ValueTask<int> SendAsync(ReadOnlyMemory<byte> datagram, IPEndPoint endPoint, CancellationToken cancellationToken)
        {
            UdpReceiveResult receiveResult = new UdpReceiveResult(datagram.ToArray(), LocalEndpoint);
            _mock.Transmit(receiveResult);
            return ValueTask.FromResult(datagram.Length);
        }
    }
}
