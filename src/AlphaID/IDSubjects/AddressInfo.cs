using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IDSubjects;

/// <summary>
/// 表示配送地址。
/// </summary>
[Owned]
public class AddressInfo
{
    /// <summary>
    /// 国家。
    /// </summary>
    [MaxLength(50)]
    public string Country { get; set; } = default!;

    /// <summary>
    /// 地区/省
    /// </summary>
    [MaxLength(50)]
    public string? State { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [MaxLength(50)]
    public string City { get; set; } = default!;

    /// <summary>
    /// 街道1
    /// </summary>
    [MaxLength(50)]
    public string Street1 { get; set; } = default!;

    /// <summary>
    /// 街道2
    /// </summary>
    [MaxLength(50)]
    public string? Street2 { get; set; }

    /// <summary>
    /// 街道3
    /// </summary>
    [MaxLength(50)]
    public string? Street3 { get; set; }

    /// <summary>
    /// 公司
    /// </summary>
    [MaxLength(50)]
    public string? Company { get; set; }

    /// <summary>
    /// 收件人
    /// </summary>
    [MaxLength(50)]
    public string Receiver { get; set; } = default!;

    /// <summary>
    /// 收件人联系方式。
    /// </summary>
    [MaxLength(20), Unicode(false)]
    public string Contact { get; set; } = default!;

    /// <summary>
    /// 邮政编号。
    /// </summary>
    [MaxLength(20), Unicode(false)]
    public string? PostalCode { get; set; }
}