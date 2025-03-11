using System.Net;

namespace Flexinets.Net
{
    public interface IUdpClientFactory
    {
        IUdpClient CreateClient(IPEndPoint localEndpoint);
    }
}
