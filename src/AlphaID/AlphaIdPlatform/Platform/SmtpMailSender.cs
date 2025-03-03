using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;

namespace AlphaIdPlatform.Platform;

/// <summary>
/// Email Sender.
/// </summary>
/// <remarks>
/// Initialize Email sender via options and logger.
/// </remarks>
/// <param name="options">Options to configure this mail sender.</param>
/// <param name="logger">Logger for logging of this mail sender.</param>
public class SmtpMailSender(IOptions<SmtpMailSenderOptions> options, ILogger<SmtpMailSender>? logger) : IEmailSender
{
    private readonly SmtpMailSenderOptions _options = options.Value;

    /// <summary>
    /// Send email to specified recipient with subject and html message body.
    /// </summary>
    /// <param name="email">Recipient.</param>
    /// <param name="subject">Title of this mail.</param>
    /// <param name="htmlMessage">HTML mail body.</param>
    /// <returns>Task self.</returns>
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using var smtpClient = new SmtpClient(_options.Server, _options.Port);
        if (_options.UseDefaultCredentials)
        {
            smtpClient.UseDefaultCredentials = true;
        }
        else
        {
            var cred = new NetworkCredential(_options.UserName, _options.Password);
            smtpClient.Credentials = cred;
        }

        var to = new MailAddress(email);
        var from = new MailAddress(_options.FromMailAddress, _options.FromDisplayName, Encoding.UTF8);
        var message = new MailMessage(from, to)
        {
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        try
        {
            await smtpClient.SendMailAsync(message);
            logger?.LogInformation("已向{email}发送了标题为'{subject}'的邮件。", to.Address, subject);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "发送邮件时发生异常，消息是{message}", ex.Message);
            throw;
        }
    }
}