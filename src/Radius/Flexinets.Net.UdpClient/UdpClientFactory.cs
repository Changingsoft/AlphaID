using System.Net;

namespace Flexinets.Net
{
    public class UdpClientFactory : IUdpClientFactory
    {
        public IUdpClient CreateClient(IPEndPoint localEndpoint)
        {
            return new UdpClientWrapper(localEndpoint);
        }
    }
}
