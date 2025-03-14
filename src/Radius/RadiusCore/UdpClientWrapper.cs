using System.Net;
using System.Net.Sockets;

namespace RadiusCore;

/// <summary>
/// Wrapper for System.Net.UdpClient
/// Only a subset of the methods are currently supported
/// </summary>
public class UdpClientWrapper(IPEndPoint localEndpoint) : IUdpClient
{
    private readonly UdpClient _client = new(localEndpoint);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <param name="length"></param>
    /// <param name="remoteEndpoint"></param>
    /// <returns></returns>
    public Task<int> SendAsync(byte[] content, int length, IPEndPoint remoteEndpoint)
    {
        return _client.SendAsync(content, length, remoteEndpoint);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public ValueTask<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken)
    {
        return _client.ReceiveAsync(cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}