namespace RadiusCore;

/// <summary>
/// 网络策略。
/// </summary>
public class NetworkPolicy
{
    /// <summary>
    /// Id。
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 策略名称。
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 应用顺序。
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// 是否启用。
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 访问权限。
    /// </summary>
    public AccessPermission AccessPermission { get; set; }

    /// <summary>
    /// 忽略用户账户的拨入属性。
    /// </summary>
    public bool IgnoreDailInProperties { get; set; } = false;

    /// <summary>
    /// 网络连接方法。
    /// 选择向NPS发送连接请求的网络访问服务器类型。你可以选择网络访问服务器的类型或特定于供应商的类型，也可以不选择。
    /// 如果你的网络访问服务器是802.1X身份验证交换机或无线访问点，请选择未指定。
    /// </summary>
    public string? ConnectionMethod { get; set; }

    /// <summary>
    /// 条件集合。
    /// </summary>
    public virtual ICollection<RadiusCondition> Conditions { get; set; } = new List<RadiusCondition>();

    /// <summary>
    /// 身份验证方法。
    /// </summary>
    public string AuthenticationMethod { get; set; }

    /// <summary>
    /// 空闲超时。指定在断开连接前，服务器可以保持空闲的最长时间（分钟）。
    /// </summary>
    public int? IdleTimeout { get; set; }

    /// <summary>
    /// 会话超时。指定可与用户保持连接的最长时间（分钟）。
    /// </summary>
    public int? SessionTimeout { get; set; }

    /// <summary>
    /// 被叫站ID。指定网络访问服务器的电话号码。仅允许访问此号码。
    /// </summary>
    public string? CalledStationId { get; set; }

    /// <summary>
    /// 仅允许在这些时间访问。
    /// </summary>
    public string? DateTimeLimit { get; set; }

    /// <summary>
    /// NAS端口类型。
    /// </summary>
    public string? NasPortType { get; set; }

    /// <summary>
    /// 将要发送到RADIUS客户端的属性。
    /// </summary>
    public string? ResponseAdditionalAttributes { get; set; }

    /// <summary>
    /// 将要发送到RADIUS客户端的供应商特定属性。
    /// </summary>
    public string? ResponseAdditionalVendorSpec { get; set; }

    /// <summary>
    /// 多链路和带宽分配协议。
    /// </summary>
    public string? Bap { get; set; }

    /// <summary>
    /// IP筛选器。
    /// </summary>
    public string? IpFilter { get; set; }

    /// <summary>
    /// 加密选项。
    /// </summary>
    public string? Encrypt { get; set; }

    /// <summary>
    /// IP设置。
    /// </summary>
    public string? IpSettings { get; set; }
}

/// <summary>
/// 访问权限。
/// </summary>
public enum AccessPermission
{
    /// <summary>
    /// 授予访问。
    /// </summary>
    Grant,
    /// <summary>
    /// 拒绝访问。
    /// </summary>
    Deny
}
