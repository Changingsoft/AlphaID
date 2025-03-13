using System.Net;

namespace RadiusCore
{
    /// <summary>
    /// Mock IUdpClientFactory which returns a singleton mock IUdpClient
    /// </summary>
    /// <remarks>
    /// Create factory with specified client
    /// </remarks>
    /// <param name="mockClient"></param>
    public class UdpClientMockFactory(IUdpClient mockClient) : IUdpClientFactory
    {
        /// <summary>
        /// Get the singleton IUdpClient
        /// </summary>
        /// <param name="localEndpoint"></param>
        /// <returns></returns>
        public IUdpClient CreateClient(IPEndPoint localEndpoint)
        {
            return mockClient;
        }
    }
}
