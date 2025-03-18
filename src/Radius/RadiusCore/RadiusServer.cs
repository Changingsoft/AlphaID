using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RadiusCore.Packet;
using RadiusCore.RadiusConstants;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace RadiusCore;

/// <summary>
/// RADIUS server.
/// </summary>
/// <param name="radiusPacketParser"></param>
/// <param name="packetHandlerRepository"></param>
/// <param name="options">Options for RADIUS server.</param>
/// <param name="logger">Logger for RADIUS server.</param>
public class RadiusServer(
    IServiceProvider serviceProvider,
    IOptions<RadiusServerOptions> options,
    ILogger<RadiusServer>? logger) : IHostedService, IDisposable
{
    private UdpClient? _udpClient;
    private Task? _receiveLoopTask;
    private CancellationTokenSource? _stoppingCts;
    private readonly ConnectionRequestHandlerFactory _connectionRequestHandlerFactory;
    private readonly NetworkPolicyHandlerFactory networkPolicyFactory;

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

        _udpClient = new UdpClient(localEndpoint);

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
            await _stoppingCts!.CancelAsync();
        }
        finally
        {
            await _receiveLoopTask.WaitAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
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
                UdpReceiveResult result = await _udpClient!.ReceiveAsync(cancellationToken);
                if (logger != null && logger.IsEnabled(LogLevel.Trace))
                    logger?.LogTrace("Receive data:{data}", result.Buffer);
                
                logger?.LogDebug("Received packet from {result.RemoteEndPoint}", result.RemoteEndPoint);
                await Task.Run(async () =>
                {
                    using var scope = Services.CreateScope();
                    //读取报文并创建处理上下文
                    RadiusPacketDataStruct radiusPacketStruct =
                        RadiusPacketDataStructExtensions.FromByteArray(result.Buffer, out var attributes);

                    RadiusRequest request = new(radiusPacketStruct, attributes, result.RemoteEndPoint);

                    RadiusContext radiusContext = new(request, this, scope.ServiceProvider);

                    //处理报文
                    var connectionRequestHandler = _connectionRequestHandlerFactory.CreateHandler(radiusContext);
                    await connectionRequestHandler.HandleAsync();

                    var networkPolicyHandler = networkPolicyFactory.CreateHandler(radiusContext);
                    await networkPolicyHandler.HandleAsync();

                    //处理完毕后，发送响应报文
                    await _udpClient.SendAsync([0x10, 0x10], 2, radiusContext.Request.Remote);

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
        _udpClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}