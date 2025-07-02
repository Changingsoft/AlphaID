using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaIdPlatform.Identity;

/// <summary>
/// 银行账户。
/// </summary>
[Owned]
[Table("NaturalPersonBankAccount")]
[PrimaryKey(nameof(AccountNumber), nameof(NaturalPersonId))]
public class NaturalPersonBankAccount
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
    public string? AccountName { get; set; }

    /// <summary>
    /// 开户行
    /// </summary>
    [MaxLength(100)]
    public string? BankName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string NaturalPersonId { get; set; } = null!;
}