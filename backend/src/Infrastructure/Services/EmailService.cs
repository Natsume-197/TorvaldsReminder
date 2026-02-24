using MailKit.Net.Smtp;
using MimeKit;
using TorvaldsReminder.Application.Interfaces;
using TorvaldsReminder.Infrastructure.Settings;

namespace TorvaldsReminder.Infrastructure.Services;

public class EmailService(EmailSettings settings) : IEmailService
{
    public async Task SendReminderAsync(string toEmail, string clientName, string number, string nextStatus)
    {
        var msg = new MimeMessage();
        msg.From.Add(MailboxAddress.Parse(settings.From));
        msg.To.Add(MailboxAddress.Parse(toEmail));
        msg.Subject = $"Aviso sobre factura {number}";
        msg.Body = new TextPart("html")
        {
            Text = $"""
                <p>Estimado(a) <strong>{clientName}</strong>,</p>
                <p>Su factura <strong>{number}</strong> ha pasado al estado de <strong>{nextStatus}</strong>.</p>
                """
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(settings.Host, settings.Port, false);
        if (!string.IsNullOrEmpty(settings.Username))
            await smtp.AuthenticateAsync(settings.Username, settings.Password);
        await smtp.SendAsync(msg);
        await smtp.DisconnectAsync(true);
    }
}
