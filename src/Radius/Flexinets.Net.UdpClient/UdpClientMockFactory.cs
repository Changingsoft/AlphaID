using System.Net;

namespace Flexinets.Net
{
    /// <summary>
    /// Mock IUdpClientFactory which returns a singleton mock IUdpClient
    /// </summary>
    public class UdpClientMockFactory : IUdpClientFactory
    {
        private IUdpClient _mockClient;


        /// <summary>
        /// Create factory with specified client
        /// </summary>
        /// <param name="mockClient"></param>
        public UdpClientMockFactory(IUdpClient mockClient)
        {
            _mockClient = mockClient;
        }


        /// <summary>
        /// Get the singleton IUdpClient
        /// </summary>
        /// <param name="localEndpoint"></param>
        /// <returns></returns>
        public IUdpClient CreateClient(IPEndPoint localEndpoint)
        {
            return _mockClient;
        }
    }
}
