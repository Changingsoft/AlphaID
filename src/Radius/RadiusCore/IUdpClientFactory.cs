using System.Net;

namespace RadiusCore
{
    public interface IUdpClientFactory
    {
        IUdpClient CreateClient(IPEndPoint localEndpoint);
    }
}
