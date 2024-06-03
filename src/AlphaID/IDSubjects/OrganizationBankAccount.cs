using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdSubjects;

/// <summary>
///     银行账户。
/// </summary>
[Table("OrganizationBankAccount")]
[PrimaryKey(nameof(AccountNumber), nameof(OrganizationId))]
public class OrganizationBankAccount
{
    /// <summary>
    /// </summary>
    protected OrganizationBankAccount()
    {
    }

    /// <summary>
    ///     通过银行信息初始化银行账号。
    /// </summary>
    /// <param name="organization"></param>
    /// <param name="accountNumber"></param>
    /// <param name="accountName"></param>
    /// <param name="bankName"></param>
    internal OrganizationBankAccount(GenericOrganization organization,
        string accountNumber,
        string accountName,
        string? bankName)
    {
        AccountNumber = accountNumber;
        AccountName = accountName;
        BankName = bankName;
        Organization = organization;
        OrganizationId = organization.Id;
    }

    /// <summary>
    ///     账号
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string AccountNumber { get; set; } = default!;

    /// <summary>
    ///     户名
    /// </summary>
    [MaxLength(100)]
    public string AccountName { get; set; } = default!;

    /// <summary>
    ///     开户行
    /// </summary>
    [MaxLength(100)]
    public string? BankName { get; set; }

    /// <summary>
    /// </summary>
    [MaxLength(20)]
    public string? Usage { get; set; }

    /// <summary>
    /// </summary>
    public bool Default { get; protected internal set; } = false;

    /// <summary>
    ///     主体Id.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string OrganizationId { get; protected set; } = default!;

    /// <summary>
    /// </summary>
    [ForeignKey(nameof(OrganizationId))]
    public GenericOrganization Organization { get; protected set; } = default!;
}