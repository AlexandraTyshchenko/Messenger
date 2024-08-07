using MailKit.Security;
using Messenger.Business.Interfaces;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using Messenger.Business.Options;
using Microsoft.Extensions.Options;

namespace Messenger.Business.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public async Task SendEmailAsync(string userEmail, string subject, string message)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_smtpSettings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(userEmail));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = $"<h1>{message}</h1>" };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_smtpSettings.Username,_smtpSettings.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
