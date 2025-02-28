namespace AlphaIdPlatform;

/// <summary>
/// 系统URL选项。
/// </summary>
public class SystemUrlInfo
{
    /// <summary>
    /// </summary>
    public Uri AdminWebAppUrl { get; set; } = new("https://localhost:49728");

    /// <summary>
    /// </summary>
    public Uri WebApiUrl { get; set; } = new("https://localhost:49727");

    /// <summary>
    /// </summary>
    public Uri AuthCenterUrl { get; set; } = new("https://localhost:49726");
}