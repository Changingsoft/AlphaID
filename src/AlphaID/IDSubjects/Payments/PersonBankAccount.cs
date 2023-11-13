using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDSubjects.Payments;

/// <summary>
/// 银行账户。
/// </summary>
[Table("PersonBankAccount")]
[PrimaryKey(nameof(AccountNumber), nameof(PersonId))]
public class PersonBankAccount
{
    /// <summary>
    /// 
    /// </summary>
    protected internal PersonBankAccount() { }

    /// <summary>
    /// 通过银行信息初始化银行账号。
    /// </summary>
    /// <param name="accountNumber"></param>
    /// <param name="accountName"></param>
    /// <param name="bankName"></param>
    public PersonBankAccount(string accountNumber, string? accountName, string? bankName)
    {
        this.AccountNumber = accountNumber;
        this.AccountName = accountName;
        this.BankName = bankName;
    }

    /// <summary>
    /// 账号
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string AccountNumber { get; set; } = default!;

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
    /// 主体Id.
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string PersonId { get; protected internal set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey(nameof(PersonId))]
    public virtual NaturalPerson Person { get; protected internal set; } = default!;
}
