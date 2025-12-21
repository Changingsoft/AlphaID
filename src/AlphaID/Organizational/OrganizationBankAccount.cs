namespace Organizational;

/// <summary>
/// 银行账户。
/// </summary>
public class OrganizationBankAccount
{
    /// <summary>
    /// 账号
    /// </summary>
    public string AccountNumber { get; set; } = null!;

    /// <summary>
    /// 户名
    /// </summary>
    public string AccountName { get; set; } = null!;

    /// <summary>
    /// 开户行
    /// </summary>
    public string? BankName { get; set; }

    /// <summary>
    /// </summary>
    public string? Usage { get; set; }

    /// <summary>
    /// </summary>
    public bool Default { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    public string OrganizationId { get; set; } = null!;
}