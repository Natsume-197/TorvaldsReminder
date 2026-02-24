using TorvaldsReminder.Domain.Entities;
using TorvaldsReminder.Domain.Enums;

namespace TorvaldsReminder.Application.Interfaces;

public interface IInvoiceRepository
{
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task<IEnumerable<Invoice>> GetPendingAsync();
    Task UpdateStatusAsync(string invoiceId, InvoiceStatus newStatus);
}