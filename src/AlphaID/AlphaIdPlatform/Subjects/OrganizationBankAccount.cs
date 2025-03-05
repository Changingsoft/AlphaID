using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AlphaIdPlatform.Subjects;

/// <summary>
/// 银行账户。
/// </summary>
[Owned]
[Table("OrganizationBankAccount")]
[PrimaryKey(nameof(AccountNumber), nameof(OrganizationId))]
public class OrganizationBankAccount
{
    /// <summary>
    /// 账号
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string AccountNumber { get; set; } = null!;

    /// <summary>
    /// 户名
    /// </summary>
    [MaxLength(100)]
    public string AccountName { get; set; } = null!;

    /// <summary>
    /// 开户行
    /// </summary>
    [MaxLength(100)]
    public string? BankName { get; set; }

    /// <summary>
    /// </summary>
    [MaxLength(20)]
    public string? Usage { get; set; }

    /// <summary>
    /// </summary>
    public bool Default { get; set; } = false;

    public string OrganizationId { get; set; } = null!;
}