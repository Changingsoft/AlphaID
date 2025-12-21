
namespace IdSubjects;

/// <summary>
/// 表示配送地址。
/// </summary>
public record AddressInfo
{
    /// <summary>
    /// 国家。
    /// </summary>
    public string Country { get; set; } = null!;

    /// <summary>
    /// 地区/省
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string Locality { get; set; } = null!;

    /// <summary>
    /// 街道1
    /// </summary>
    public string Street1 { get; set; } = null!;

    /// <summary>
    /// 街道2
    /// </summary>
    public string? Street2 { get; set; }

    /// <summary>
    /// 街道3
    /// </summary>
    public string? Street3 { get; set; }

    /// <summary>
    /// 收件人
    /// </summary>
    public string? Recipient { get; set; }

    /// <summary>
    /// 收件人联系方式。
    /// </summary>
    public string? Contact { get; set; }

    /// <summary>
    /// 邮政编号。
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Recipient} ,{Street3},{Street2},{Street1},{Locality},{Country},{PostalCode}";
    }
}