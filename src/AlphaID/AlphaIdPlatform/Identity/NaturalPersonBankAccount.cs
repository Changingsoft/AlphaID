
namespace AlphaIdPlatform.Identity;

/// <summary>
/// 银行账户。
/// </summary>
public class NaturalPersonBankAccount
{
    /// <summary>
    /// 账号
    /// </summary>
    public string AccountNumber { get; set; } = null!;

    /// <summary>
    /// 户名
    /// </summary>
    public string? AccountName { get; set; }

    /// <summary>
    /// 开户行
    /// </summary>
    public string? BankName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string NaturalPersonId { get; set; } = null!;
}