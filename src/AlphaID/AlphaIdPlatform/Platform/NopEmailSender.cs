namespace AlphaIdPlatform.Platform;

/// <summary>
/// 一个无实际操作的邮件发送器，用于调试阶段模拟邮件发送。该发送器不会向外部发送邮件，但会在日志中记录一条发送邮件内容的信息。
/// </summary>
/// <remarks>
/// </remarks>
/// <param name="logger"></param>
public class NopEmailSender(ILogger<NopEmailSender> logger) : IEmailSender
{
    /// <summary>
    /// </summary>
    /// <param name="email"></param>
    /// <param name="subject"></param>
    /// <param name="htmlMessage"></param>
    /// <returns></returns>
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        logger.LogInformation("已模拟向<{email}>发送了主题为“{subject}”的邮件，内容是：{mailContent}", email, subject, htmlMessage);
        return Task.CompletedTask;
    }
}