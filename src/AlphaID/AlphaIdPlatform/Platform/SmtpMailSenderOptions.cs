namespace AlphaIdPlatform.Platform;

/// <summary>
/// Options for Email Sender.
/// </summary>
public class SmtpMailSenderOptions
{
    /// <summary>
    /// SMTP Server.
    /// </summary>
    public string Server { get; set; } = null!;

    /// <summary>
    /// SMTP Protocol Port. Use 25 by default.
    /// </summary>
    public int Port { get; set; } = 25;

    /// <summary>
    /// UserName.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 获取或设置一个值，让邮件发送器是否使用默认凭据。默认为true。
    /// 如果计划使用Web应用程序的托管池账户身份作为凭据，请将此设为true，否则应提供用户名和密码。
    /// </summary>
    public bool UseDefaultCredentials { get; set; } = true;

    /// <summary>
    /// 发送人电子邮件地址。
    /// </summary>
    public string FromMailAddress { get; set; } = null!;

    /// <summary>
    /// 发送人显示名称。
    /// </summary>
    public string FromDisplayName { get; set; } = null!;
}