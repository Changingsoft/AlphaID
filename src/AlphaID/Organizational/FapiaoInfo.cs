namespace Organizational;

/// <summary>
/// 发票信息
/// </summary>
public record FapiaoInfo
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 纳税人识别号
    /// </summary>
    public string TaxPayerId { get; set; } = null!;

    /// <summary>
    /// 地址
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// 联系电话
    /// </summary>
    public string Contact { get; set; } = null!;

    /// <summary>
    /// 银行名
    /// </summary>
    public string Bank { get; set; } = null!;

    /// <summary>
    /// 账号
    /// </summary>
    public string Account { get; set; } = null!;
}