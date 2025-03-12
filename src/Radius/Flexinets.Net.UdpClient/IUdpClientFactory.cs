using System.Net;

namespace UdpClient
{
    public interface IUdpClientFactory
    {
        IUdpClient CreateClient(IPEndPoint localEndpoint);
    }
}
