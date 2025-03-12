using System.Net;
using System.Net.Sockets;

namespace UdpClient
{
    public interface IUdpClient : IDisposable
    {
        Socket Client { get; }

        void Close();
        void Send(byte[] content, int length, IPEndPoint recipient);
        Task<int> SendAsync(byte[] content, int length, IPEndPoint remoteEndpoint);
        Task<UdpReceiveResult> ReceiveAsync();
    }
}