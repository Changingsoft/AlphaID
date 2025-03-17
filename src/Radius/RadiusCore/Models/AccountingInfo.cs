using Microsoft.EntityFrameworkCore;

namespace RadiusCore.Models;

/// <summary>
/// 记账设置。
/// </summary>
[Owned]
public class AccountingInfo
{
    /// <summary>
    /// 端口号。
    /// </summary>
    public int Port { get; set; } = 1813;

    /// <summary>
    /// 共享密钥。如果为null，表示使用与认证服务器相同的共享密钥。
    /// </summary>
    public string? SharedSecret { get; set; }

    /// <summary>
    /// 将网络访问服务器启动和停止通知转发到此服务器。
    /// </summary>
    public bool NotifyServerStartAndStop { get; set; }
}