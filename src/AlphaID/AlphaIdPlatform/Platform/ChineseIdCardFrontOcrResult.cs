namespace AlphaIdPlatform.Platform;

/// <summary>
/// Result of ID Card Front.
/// </summary>
public class ChineseIdCardFrontOcrResult
{
    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 性别，可能的值为：["男"|"女"]
    /// </summary>
    public string SexString { get; set; } = null!;

    /// <summary>
    /// 民族
    /// </summary>
    public string Nationality { get; set; } = null!;

    /// <summary>
    /// 出生日期字符串
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// 住址
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// 身份证号码。
    /// </summary>
    public string IdCardNumber { get; set; } = null!;
}