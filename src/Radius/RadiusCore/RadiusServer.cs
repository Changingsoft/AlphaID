using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RadiusCore.Packet;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace RadiusCore;

/// <summary>
/// RADIUS server.
/// </summary>
/// <param name="options">Options for RADIUS server.</param>
/// <param name="logger">Logger for RADIUS server.</param>
public class RadiusServer(
    IUdpClient udpClient,
    RadiusRequestParser packetParser,
    IServiceProvider serviceProvider,
    IOptions<RadiusServerOptions> options,
    ILogger<RadiusServer>? logger) : IHostedService, IDisposable
{
    private Task? _receiveLoopTask;
    private CancellationTokenSource? _stoppingCts;
    private readonly ConnectionRequestHandlerFactory _connectionRequestHandlerFactory;
    private readonly NetworkPolicyHandlerFactory networkPolicyFactory;
    SessionCache _sessionCache = new();

    /// <summary>
    /// 
    /// </summary>
    public IServiceProvider Services { get; } = serviceProvider;

    /// <summary>
    /// Start listening for requests
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var localEndpoint = new IPEndPoint(IPAddress.Any, options.Value.AuthenticationServerPort);
        logger?.LogInformation("Starting Radius server on {localEndpoint}", localEndpoint);
        
        _receiveLoopTask = ReceiveLoopTask(_stoppingCts.Token);

        if (_receiveLoopTask.IsCompleted)
            return _receiveLoopTask;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stop listening
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_receiveLoopTask == null)
            return;

        try
        {
            logger?.LogInformation("Stopping Radius server...");
            await _stoppingCts!.CancelAsync();
        }
        finally
        {
            await _receiveLoopTask.WaitAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
            logger?.LogInformation("Radius server stopped.");
        }
    }

    /// <summary>
    /// Start the loop used for receiving packets
    /// </summary>
    /// <returns></returns>
    private async Task ReceiveLoopTask(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                logger?.LogDebug("Server now ready to receive packet");
                UdpReceiveResult result = await udpClient.ReceiveAsync(cancellationToken);

                if (logger != null && logger.IsEnabled(LogLevel.Trace))
                    logger?.LogTrace("Receive data:{data}", result.Buffer);

                logger?.LogDebug("Received packet from {result.RemoteEndPoint}", result.RemoteEndPoint);
                await Task.Run(async () =>
                {
                    Stopwatch watch = Stopwatch.StartNew();
                    using var scope = Services.CreateScope();

                    RadiusPacket packet;
                    try
                    {
                        packet = RadiusPacket.FromByteArray(result.Buffer);
                    }
                    catch (Exception e)
                    {
                        logger?.LogError(e, "从远端{remote}收到了损坏的数据包。", result.RemoteEndPoint);
                        return;
                    }

                    RadiusRequest request = new RadiusRequest(packet, result.RemoteEndPoint);

                    //检查是否为重复数据包
                    if (_sessionCache.Contains($"{result.RemoteEndPoint}/{packet.Identifier}"))
                    {
                        logger?.LogWarning("Duplicate packet detected from {result.RemoteEndPoint}, ignoring.", result.RemoteEndPoint);
                        return;
                    }
                    _sessionCache.Set($"{result.RemoteEndPoint}/{packet.Identifier}", new object());

                    RadiusContext radiusContext = new(request, this, scope.ServiceProvider);

                    //处理连接请求策略
                    var connectionRequestHandler = _connectionRequestHandlerFactory.CreateHandler(radiusContext);
                    await connectionRequestHandler.HandleAsync(radiusContext);

                    var networkPolicyHandler = networkPolicyFactory.CreateHandler(radiusContext);
                    await networkPolicyHandler.HandleAsync(radiusContext);

                    //处理完毕后，发送响应报文
                    await udpClient.SendAsync(new ReadOnlyMemory<byte>([0x10, 0x10]), radiusContext.Request.Remote, cancellationToken);
                    watch.Stop();
                    logger?.LogDebug("数据包已处理，用时{ms}毫秒。", watch.ElapsedMilliseconds);
                    if (watch.ElapsedMilliseconds > 3000)
                    {
                        logger?.LogWarning("数据包处理时间过长，用时{ms}毫秒。", watch.ElapsedMilliseconds);
                    }

                }, cancellationToken)
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            logger?.LogError(task.Exception, "Error handling packet");
                        }
                    }, TaskContinuationOptions.OnlyOnFaulted);

            }
            catch (OperationCanceledException)
            {
                logger?.LogError("服务已被取消。");
            }
            catch (Exception ex)
            {
                logger?.LogCritical(ex, "Something went wrong receiving packet");
            }
        }
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        _stoppingCts?.Cancel();
        GC.SuppressFinalize(this);
    }

    internal async Task SendAsync(RadiusResponse response)
    {
        await udpClient.SendAsync(response.ToBytes(), response.Remote!, CancellationToken.None);
    }
}