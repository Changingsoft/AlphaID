using RadiusCore.Models;

namespace RadiusCore;

/// <summary>
/// RADIUS上下文。
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="request"></param>
/// <param name="server"></param>
public class RadiusContext(RadiusRequest request, RadiusServer server, IServiceProvider services)
{
    /// <summary>
    /// 
    /// </summary>
    public RadiusRequest Request { get; } = request;

    /// <summary>
    /// 
    /// </summary>
    public RadiusServer Server { get; } = server;

    /// <summary>
    /// Gets the service provider that scopes the lifetime of the RADIUS context.
    /// </summary>
    public IServiceProvider Services { get; } = services;

    /// <summary>
    /// 
    /// </summary>
    public RadiusResponse Response { get; } = new();

    /// <summary>
    /// 当前连接请求策略。
    /// </summary>
    public ConnectionRequestPolicy? ConnectionRequestPolicy { get; internal set; }

    /// <summary>
    /// 当前网络策略。
    /// </summary>
    public NetworkPolicy? NetworkPolicy { get; internal set; }

    /// <summary>
    /// 当前客户端。
    /// </summary>
    public RadiusClient? Client { get; internal set; }
}