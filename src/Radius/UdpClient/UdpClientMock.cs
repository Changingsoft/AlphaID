using System.Net;
using System.Net.Sockets;

namespace UdpClient
{
    public class UdpClientMock : IUdpClient
    {
        private TaskCompletionSource<UdpReceiveResult> _receiveTaskCompletionSource;
        private TaskCompletionSource<UdpReceiveResult> _sendTaskCompletionSource;


        public Socket Client => null;

        public void Close()
        {
            // Ok...
        }

        public void Dispose()
        {
            // Sure...            
        }

        public async Task<UdpReceiveResult> ReceiveAsync()
        {
            _receiveTaskCompletionSource = new TaskCompletionSource<UdpReceiveResult>();
            return await _receiveTaskCompletionSource.Task;
        }

        public void Send(byte[] content, int length, IPEndPoint recipient)
        {
            _sendTaskCompletionSource.SetResult(new UdpReceiveResult(content, recipient));
        }

        public Task<int> SendAsync(byte[] content, int length, IPEndPoint remoteEndpoint)
        {
            Send(content, length, remoteEndpoint);
            return Task.FromResult(0);
        }

        public Task<UdpReceiveResult> SendMock(UdpReceiveResult mockResult)
        {
            _sendTaskCompletionSource = new TaskCompletionSource<UdpReceiveResult>();
            _receiveTaskCompletionSource.SetResult(mockResult);
            return _sendTaskCompletionSource.Task;
        }
    }
}
