using System.ComponentModel.DataAnnotations;

namespace RadiusCore.Models;

/// <summary>
/// Radius客户端。
/// </summary>
public class RadiusClient
{
    /// <summary>
    /// 客户端的唯一标识符。
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 客户端的友好名称。
    /// </summary>
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 客户端的IP地址。
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// Radius客户端的共享密钥。
    /// </summary>
    [MaxLength(50)]
    public string SharedSecret { get; set; } = null!;

    /// <summary>
    /// 客户端是否启用。默认为true。
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 供应商名称。默认为“RADIUS Standard”。
    /// 为大多数RADIUS客户端指定RADIUS标准，或从列表中选择RADIUS客户端供应商。
    /// </summary>
    [MaxLength(50)]
    public string Vender { get; set; } = "RADIUS Standard";

    /// <summary>
    /// Accept-Request消息必须包含Message-Authenticator属性。默认为false。
    /// </summary>
    public bool IncludeMessageAuthenticatorAttribute { get; set; } = false;
}