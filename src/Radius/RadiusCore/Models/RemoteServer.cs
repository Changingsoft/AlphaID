using Microsoft.EntityFrameworkCore;

namespace RadiusCore.Models;

/// <summary>
/// 远程服务器。
/// </summary>
[Owned]
public class RemoteServer
{
    /// <summary>
    /// Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 服务器地址。
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// 端口号。
    /// </summary>
    public int Port { get; set; } = 1812;

    /// <summary>
    /// 共享密钥。
    /// </summary>
    public string SharedSecret { get; set; } = null!;

    /// <summary>
    /// 请求必须包含Message-Authenticator属性。
    /// </summary>
    public bool IncludeMessageAuthenticatorAttribute { get; set; } = false;

    /// <summary>
    /// 记账设置。
    /// </summary>
    public virtual AccountingInfo Accounting { get; set; } = new();

    /// <summary>
    /// 优先级。值越小，优先级越高。
    /// </summary>
    public int Poried { get; set; } = 1;

    /// <summary>
    /// 权重。值越大，权重越高。
    /// </summary>
    public int Weight { get; set; } = 50;

    /// <summary>
    /// 在这些秒数后没有反应，就认为请求被放弃。
    /// </summary>
    public int Timeout { get; set; } = 3;

    /// <summary>
    /// 服务被识别为不可用之前，被放弃请求的最大数目。
    /// </summary>
    public int MaxDropRequestCount { get; set; } = 5;

    /// <summary>
    /// 当服务器被识别为不可用时，请求之间的间隔秒数。
    /// </summary>
    public int IntervalWhenUnavailable { get; set; } = 30;
}