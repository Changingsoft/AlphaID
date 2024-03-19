namespace IdSubjects.Payments;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="accountNumber"></param>
/// <param name="accountName"></param>
/// <param name="bankName"></param>
public class BankAccountInfo(string accountNumber, string accountName, string bankName)
{

    /// <summary>
    /// 账号
    /// </summary>
    public string AccountNumber { get; set; } = accountNumber;

    /// <summary>
    /// 账户名称。
    /// </summary>
    public string AccountName { get; set; } = accountName;

    /// <summary>
    /// 银行名称
    /// </summary>
    public string BankName { get; set; } = bankName;
}