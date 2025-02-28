namespace AlphaIdPlatform;

/// <summary>
/// 产品信息。
/// </summary>
public class ProductInfo
{
    /// <summary>
    /// 产品名称。
    /// </summary>
    public string Name { get; set; } = "Alpha ID";

    /// <summary>
    /// 运营者组织。
    /// </summary>
    public string Organization { get; set; } = "Your Organization";

    /// <summary>
    /// </summary>
    public string Copyright { get; set; } = "2023 Your Organization, All rights reserved.";

    /// <summary>
    /// Trademark
    /// </summary>
    public string Trademark { get; set; } = "Your Trademark";

    /// <summary>
    /// Favicon path.
    /// </summary>
    public string FavIconPath { get; set; } = "/favicon.ico";

    /// <summary>
    /// Logo image path.
    /// </summary>
    public string LogoImagePath { get; set; } = "/logo.png";
}