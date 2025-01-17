namespace AlphaIdPlatform.Payments;

/// <summary>
/// </summary>
/// <remarks>
/// </remarks>
/// <param name="AccountNumber"></param>
/// <param name="AccountName"></param>
/// <param name="BankName"></param>
public record BankAccountInfo(string AccountNumber, string AccountName, string BankName)
{
    /// <summary>
    ///     账号
    /// </summary>
    public string AccountNumber { get; set; } = AccountNumber;

    /// <summary>
    ///     账户名称。
    /// </summary>
    public string AccountName { get; set; } = AccountName;

    /// <summary>
    ///     银行名称
    /// </summary>
    public string BankName { get; set; } = BankName;
}