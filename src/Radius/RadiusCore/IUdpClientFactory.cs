using System.Net;

namespace RadiusCore;

/// <summary>
/// 
/// </summary>
public interface IUdpClientFactory
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="localEndpoint"></param>
    /// <returns></returns>
    IUdpClient CreateClient(IPEndPoint localEndpoint);
}