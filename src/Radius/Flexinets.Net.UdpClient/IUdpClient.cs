using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Flexinets.Net
{
    public interface IUdpClient : IDisposable
    {
        Socket Client { get; }

        void Close();
        void Send(Byte[] content, Int32 length, IPEndPoint recipient);
        Task<Int32> SendAsync(Byte[] content, Int32 length, IPEndPoint remoteEndpoint);
        Task<UdpReceiveResult> ReceiveAsync();
    }
}