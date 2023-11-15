namespace AlphaIdPlatform.Platform;

/// <summary>
/// 提供邮件发送能力。
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// 向指定收件人发送邮件。
    /// </summary>
    /// <param name="email">收件人。</param>
    /// <param name="subject">主题。</param>
    /// <param name="htmlMessage">支持HTML格式的邮件正文。</param>
    /// <returns></returns>
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}