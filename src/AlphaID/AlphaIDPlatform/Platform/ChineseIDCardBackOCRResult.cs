namespace AlphaIdPlatform.Platform;

/// <summary>
/// Result of IDCardBack.
/// </summary>
public class ChineseIdCardBackOcrResult
{
    /// <summary>
    /// 签发机关。
    /// </summary>
    public string Issuer { get; set; } = default!;

    /// <summary>
    /// 签发日期字符串。
    /// </summary>
    public DateTime IssueDate { get; set; }

    /// <summary>
    /// 过期日期字符串。
    /// </summary>
    public DateTime? ExpiresDate { get; set; }
}