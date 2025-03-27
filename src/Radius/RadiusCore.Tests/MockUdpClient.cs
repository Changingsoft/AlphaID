using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RadiusCore.Tests
{
    class MockUdpClient : IUdpClient
    {
        private readonly TaskCompletionSource<UdpReceiveResult> _receiveTask = new TaskCompletionSource<UdpReceiveResult>();

        public async ValueTask<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken)
        {
            return await _receiveTask.Task;
        }

        public ValueTask<int> SendAsync(ReadOnlyMemory<byte> datagram, IPEndPoint endPoint, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SetReceiveResult(UdpReceiveResult result)
        {
            _receiveTask.SetResult(result);
        }
    }
}
