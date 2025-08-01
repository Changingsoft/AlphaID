namespace AuthCenterWebApp;

/// <summary>
/// Represents the settings for a Weixin MP (Media Platform) integration.
/// </summary>
/// <remarks>This class provides configuration options for connecting to a Weixin MP service, including enabling
/// the integration and specifying authentication credentials.</remarks>
public class WeixinMpSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the feature is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the name of the Weixin MP (Media Platform).
    /// </summary>
    public string MpName { get; set; } = "default-weixin-mp";

    /// <summary>
    /// Gets or sets the application identifier.
    /// </summary>
    public string AppId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the application secret key used for authentication.
    /// </summary>
    public string AppSecret { get; set; } = string.Empty;
}
