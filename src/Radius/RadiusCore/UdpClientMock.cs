using System.Net;
using System.Net.Sockets;

namespace RadiusCore;

/// <summary>
/// 
/// </summary>
public class UdpClientMock : IUdpClient
{
    private TaskCompletionSource<UdpReceiveResult>? _receiveTaskCompletionSource;
    private TaskCompletionSource<UdpReceiveResult>? _sendTaskCompletionSource;
        
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<UdpReceiveResult> ReceiveAsync()
    {
        _receiveTaskCompletionSource = new TaskCompletionSource<UdpReceiveResult>();
        return await _receiveTaskCompletionSource.Task;
    }

    private void Send(byte[] content, int length, IPEndPoint recipient)
    {
        _sendTaskCompletionSource?.SetResult(new UdpReceiveResult(content, recipient));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <param name="length"></param>
    /// <param name="remoteEndpoint"></param>
    /// <returns></returns>
    public Task<int> SendAsync(byte[] content, int length, IPEndPoint remoteEndpoint)
    {
        Send(content, length, remoteEndpoint);
        return Task.FromResult(0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mockResult"></param>
    /// <returns></returns>
    public Task<UdpReceiveResult> SendMock(UdpReceiveResult mockResult)
    {
        _sendTaskCompletionSource = new TaskCompletionSource<UdpReceiveResult>();
        _receiveTaskCompletionSource?.SetResult(mockResult);
        return _sendTaskCompletionSource.Task;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
    }
}