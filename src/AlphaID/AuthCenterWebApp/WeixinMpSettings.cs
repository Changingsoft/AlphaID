namespace AuthCenterWebApp;

public class WeixinMpSettings
{
    public bool Enabled { get; set; }

    public string MpName { get; set; } = "default-weixin-mp";

    public string AppId { get; set; } = string.Empty;

    public string AppSecret { get; set; } = string.Empty;
}
