using FluentAssertions;
using Moq;
using TorvaldsReminder.Application.Interfaces;
using TorvaldsReminder.Application.Services;
using TorvaldsReminder.Domain.Entities;
using TorvaldsReminder.Domain.Enums;

public class InvoiceProcessingServiceTests
{
    private readonly Mock<IInvoiceRepository> _invoices = new();
    private readonly Mock<IClientRepository>  _clients  = new();
    private readonly Mock<IEmailService>       _email    = new();

    private InvoiceProcessingService Sut() => new(_invoices.Object, _clients.Object, _email.Object);

    [Fact]
    public async Task PrimerRecordatorio_ActualizaASegundoYEnviaEmail()
    {        
        // ARRANGE
        var invoice = new Invoice { Id = "1", ClientId = "c1", Number = "F-001", Status = InvoiceStatus.PrimerRecordatorio };
        var client  = new Client  { Id = "c1", Name = "Linus Torvalds", Email = "linus@kernel.org" };

        // TRAINING 
        _invoices.Setup(r => r.GetPendingAsync()).ReturnsAsync([invoice]);
        _clients .Setup(r => r.GetByIdAsync("c1")).ReturnsAsync(client);

        // ACT
        await Sut().ProcessAsync();

        // ASSERT AND VERIFY
        _email.Verify(e => e.SendReminderAsync("linus@kernel.org", "Linus Torvalds", "F-001", It.IsAny<string>()), Times.Once());
        _invoices.Verify(r => r.UpdateStatusAsync("1", InvoiceStatus.SegundoRecordatorio), Times.Once());
    }

    [Fact]
    public async Task SegundoRecordatorio_ActualizaADesactivadoYEnviaEmail()
    {
        // ARRANGE
        var invoice = new Invoice { Id = "2", ClientId = "c2", Number = "F-004", Status = InvoiceStatus.SegundoRecordatorio };
        var client  = new Client  { Id = "c2", Name = "Richard Stallman", Email = "rms@gnu.org" };

        // TRAINING
        _invoices.Setup(r => r.GetPendingAsync()).ReturnsAsync([invoice]);
        _clients .Setup(r => r.GetByIdAsync("c2")).ReturnsAsync(client);

        // ACT
        await Sut().ProcessAsync();

        // ASSERT AND VERIFY
        _email.Verify(e => e.SendReminderAsync("rms@gnu.org", "Richard Stallman", "F-004", It.IsAny<string>()), Times.Once());
        _invoices.Verify(r => r.UpdateStatusAsync("2", InvoiceStatus.Desactivado), Times.Once());
    }

    [Fact]
    public async Task ClienteNoEncontrado_NadaOcurre()
    {
        // ARRANGE.
        var invoice = new Invoice { Id = "3", ClientId = "x", Status = InvoiceStatus.PrimerRecordatorio };
        
        // TRAINING
        _invoices.Setup(r => r.GetPendingAsync()).ReturnsAsync([invoice]);
        _clients .Setup(r => r.GetByIdAsync("x")).ReturnsAsync((Client?)null);

        // ACT
        await Sut().ProcessAsync();

        // ASSERT AND VERIFY
        _email.Verify(e => e.SendReminderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        _invoices.Verify(r => r.UpdateStatusAsync(It.IsAny<string>(), It.IsAny<InvoiceStatus>()), Times.Never());
    }
}
