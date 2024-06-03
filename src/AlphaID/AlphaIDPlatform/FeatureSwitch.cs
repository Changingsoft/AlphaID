namespace AlphaIdPlatform;

/// <summary>
///     表示系统特性开关。
/// </summary>
public class FeatureSwitch
{
    /// <summary>
    ///     Key of RealNameFeature.
    /// </summary>
    public const string RealNameFeature = "ProductInfo:Feature:RealName";

    /// <summary>
    ///     Key of DirectoryAccountManagementFeature.
    /// </summary>
    public const string DirectoryAccountManagementFeature = "ProductInfo:Feature:DirectoryAccountManagement";

    /// <summary>
    ///     获取或设置一个值，指示是否启用实名制认证。
    /// </summary>
    public bool RealName { get; set; } = false;

    /// <summary>
    ///     获取或设置一个值，指示是否启用目录账户管理。
    /// </summary>
    public bool DirectoryAccountManagement { get; set; } = false;
}