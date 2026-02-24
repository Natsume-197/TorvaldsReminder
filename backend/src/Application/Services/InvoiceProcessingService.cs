using TorvaldsReminder.Application.Interfaces;
using TorvaldsReminder.Domain.Enums;

namespace TorvaldsReminder.Application.Services;

public class InvoiceProcessingService(
    IInvoiceRepository invoices,
    IClientRepository clients,
    IEmailService email)
{
    public async Task ProcessAsync()
    {
        var pending = await invoices.GetPendingAsync();

        foreach (var invoice in pending)
        {
            var client = await clients.GetByIdAsync(invoice.ClientId);
            if (client is null) continue;

            var (newStatus, message) = ResolveTransition(invoice.Status);

            await email.SendReminderAsync(client.Email, client.Name, invoice.Number, message);
            await invoices.UpdateStatusAsync(invoice.Id, newStatus);
        }
    }

    private static (InvoiceStatus, string) ResolveTransition(InvoiceStatus current) =>
        current switch
        {
            InvoiceStatus.PrimerRecordatorio  => (InvoiceStatus.SegundoRecordatorio, "ha pasado a segundo recordatorio"),
            InvoiceStatus.SegundoRecordatorio => (InvoiceStatus.Desactivado,          "va a ser desactivado"),
            _ => throw new InvalidOperationException($"Estado no procesable: {current}")
        };
}
