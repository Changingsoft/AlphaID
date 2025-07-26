namespace AlphaIdWebAPI.Models;

/// <summary>
/// </summary>
public static class PersonalDataProtectionExtensions
{
    /// <summary>
    /// 获取手机号后4位。
    /// </summary>
    /// <param name="mobile"></param>
    /// <returns></returns>
    public static string MobileSuffix(this string mobile)
    {
        return mobile[^4..];
    }
}