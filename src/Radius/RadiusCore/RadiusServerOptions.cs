namespace RadiusCore;

/// <summary>
/// 
/// </summary>
public class RadiusServerOptions
{
    /// <summary>
    /// 
    /// </summary>
    public int AuthenticationServerPort { get; set; } = 1812;

    /// <summary>
    /// 重放检测超时时间。
    /// </summary>
    public int DuplicateDetectionTimeout { get; set; } = 30;
}