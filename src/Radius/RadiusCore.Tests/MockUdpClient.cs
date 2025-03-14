using System.Net;
using System.Net.Sockets;

namespace RadiusCore.Tests;

/// <summary>
/// 
/// </summary>
public class MockUdpClient : IUdpClient
{
    private TaskCompletionSource<UdpReceiveResult>? _receiveTaskCompletionSource;
    private TaskCompletionSource<UdpReceiveResult>? _sendTaskCompletionSource;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async ValueTask<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken)
    {
        _receiveTaskCompletionSource = new TaskCompletionSource<UdpReceiveResult>();
        return await _receiveTaskCompletionSource.Task;
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
        _sendTaskCompletionSource?.SetResult(new UdpReceiveResult(content, remoteEndpoint));
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

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}