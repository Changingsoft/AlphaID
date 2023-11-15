namespace IdSubjects.Payments;

/// <summary>
/// 
/// </summary>
public class BankAccountInfo
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountNumber"></param>
    /// <param name="accountName"></param>
    /// <param name="bankName"></param>
    public BankAccountInfo(string accountNumber, string accountName, string bankName)
    {
        this.AccountNumber = accountNumber;
        this.AccountName = accountName;
        this.BankName = bankName;
    }

    /// <summary>
    /// 账号
    /// </summary>
    public string AccountNumber { get; set; }

    /// <summary>
    /// 账户名称。
    /// </summary>
    public string AccountName { get; set; }

    /// <summary>
    /// 银行名称
    /// </summary>
    public string BankName { get; set; }
}