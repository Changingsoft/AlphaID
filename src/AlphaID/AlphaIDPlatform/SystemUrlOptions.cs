namespace AlphaIDPlatform;

/// <summary>
/// 系统URL选项。
/// </summary>
public class SystemUrlOptions
{
    /// <summary>
    /// 
    /// </summary>
    public Uri AdminWebAppUrl { get; set; } = new Uri("https://localhost:61315");

    /// <summary>
    /// 
    /// </summary>
    public Uri WebApiUrl { get; set; } = new Uri("https://localhost:61316");

    /// <summary>
    /// 
    /// </summary>
    public Uri MyIdUrl { get; set; } = new Uri("https://localhost:61313");

    /// <summary>
    /// 
    /// </summary>
    public Uri AuthCenterUrl { get; set; } = new Uri("https://localhost:49726");

}
