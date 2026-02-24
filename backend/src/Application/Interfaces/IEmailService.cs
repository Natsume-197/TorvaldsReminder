namespace TorvaldsReminder.Application.Interfaces;

public interface IEmailService
{
    Task SendReminderAsync(string toEmail, string clientName, string invoiceNumber, string nextStatus);
}
