using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AlphaIdPlatform.Subjects;

/// <summary>
///     发票信息
/// </summary>
[Owned]
public record FapiaoInfo
{
    /// <summary>
    ///     名称
    /// </summary>
    [MaxLength(30)]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     纳税人识别号
    /// </summary>
    [MaxLength(30)]
    public string TaxPayerId { get; set; } = null!;

    /// <summary>
    ///     地址
    /// </summary>
    [MaxLength(30)]
    public string Address { get; set; } = null!;

    /// <summary>
    ///     联系电话
    /// </summary>
    [MaxLength(30)]
    public string Contact { get; set; } = null!;

    /// <summary>
    ///     银行名
    /// </summary>
    [MaxLength(30)]
    public string Bank { get; set; } = null!;

    /// <summary>
    ///     账号
    /// </summary>
    [MaxLength(30)]
    public string Account { get; set; } = null!;
}