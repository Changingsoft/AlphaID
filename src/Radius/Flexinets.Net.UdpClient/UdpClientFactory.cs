using System.Net;

namespace UdpClient
{
    public class UdpClientFactory : IUdpClientFactory
    {
        public IUdpClient CreateClient(IPEndPoint localEndpoint)
        {
            return new UdpClientWrapper(localEndpoint);
        }
    }
}
