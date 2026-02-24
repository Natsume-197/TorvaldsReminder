using MongoDB.Driver;
using TorvaldsReminder.Application.Interfaces;
using TorvaldsReminder.Domain.Entities;
using TorvaldsReminder.Domain.Enums;
using TorvaldsReminder.Infrastructure.Settings;

namespace TorvaldsReminder.Infrastructure.Repositories;

public class InvoiceRepository(IMongoClient mongo, MongoSettings settings) : IInvoiceRepository
{
    private readonly IMongoCollection<Invoice> _col =
        mongo.GetDatabase(settings.DatabaseName).GetCollection<Invoice>("invoices");

    public async Task<IEnumerable<Invoice>> GetAllAsync() =>
        await _col.Find(_ => true).ToListAsync();

    public async Task<IEnumerable<Invoice>> GetPendingAsync()
    {
        var filter = Builders<Invoice>.Filter.In(x => x.Status,
            [InvoiceStatus.PrimerRecordatorio, InvoiceStatus.SegundoRecordatorio]);
        return await _col.Find(filter).ToListAsync();
    }

    public async Task UpdateStatusAsync(string id, InvoiceStatus status)
    {
        var filter = Builders<Invoice>.Filter.Eq(x => x.Id, id);
        var update = Builders<Invoice>.Update.Set(x => x.Status, status);
        await _col.UpdateOneAsync(filter, update);
    }
}
