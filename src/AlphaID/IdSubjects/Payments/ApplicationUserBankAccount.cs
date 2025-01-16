using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdSubjects.Payments;

/// <summary>
///     银行账户。
/// </summary>
[Table("ApplicationUserBankAccount")]
[PrimaryKey(nameof(AccountNumber), nameof(PersonId))]
public class ApplicationUserBankAccount
{
    /// <summary>
    /// </summary>
    protected internal ApplicationUserBankAccount()
    {
    }

    /// <summary>
    ///     通过银行信息初始化银行账号。
    /// </summary>
    /// <param name="accountNumber"></param>
    /// <param name="accountName"></param>
    /// <param name="bankName"></param>
    public ApplicationUserBankAccount(string accountNumber, string? accountName, string? bankName)
    {
        AccountNumber = accountNumber;
        AccountName = accountName;
        BankName = bankName;
    }

    /// <summary>
    ///     账号
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string AccountNumber { get; set; } = null!;

    /// <summary>
    ///     户名
    /// </summary>
    [MaxLength(100)]
    public string? AccountName { get; set; }

    /// <summary>
    ///     开户行
    /// </summary>
    [MaxLength(100)]
    public string? BankName { get; set; }

    /// <summary>
    ///     主体Id.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string PersonId { get; protected internal set; } = null!;

    /// <summary>
    /// </summary>
    [ForeignKey(nameof(PersonId))]
    public virtual ApplicationUser Person { get; protected internal set; } = null!;
}