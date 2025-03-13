using System.Net;

namespace RadiusCore;

/// <summary>
/// 
/// </summary>
public class UdpClientFactory : IUdpClientFactory
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="localEndpoint"></param>
    /// <returns></returns>
    public IUdpClient CreateClient(IPEndPoint localEndpoint)
    {
        return new UdpClientWrapper(localEndpoint);
    }
}