using System.Net;
using System.Net.Sockets;

namespace RadiusCore;

/// <summary>
/// 
/// </summary>
public interface IUdpClient : IDisposable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <param name="length"></param>
    /// <param name="remoteEndpoint"></param>
    /// <returns></returns>
    Task<int> SendAsync(byte[] content, int length, IPEndPoint remoteEndpoint);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<UdpReceiveResult> ReceiveAsync();
}