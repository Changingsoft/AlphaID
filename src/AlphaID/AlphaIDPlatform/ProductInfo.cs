namespace AlphaIDPlatform;

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
    public string Organization { get; set; } = "Changingsoft";

    public string TradeMark { get; set; } = "Changingsoft";

    /// <summary>
    /// Favicon path.
    /// </summary>
    public string FavIconPath { get; set; } = "/favicon.ico";

    /// <summary>
    /// Logo image path.
    /// </summary>
    public string LogoImagePath { get; set; } = "/logo.png";
}
