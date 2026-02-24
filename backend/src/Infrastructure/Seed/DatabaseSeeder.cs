using MongoDB.Driver;
using TorvaldsReminder.Domain.Entities;
using TorvaldsReminder.Domain.Enums;
using TorvaldsReminder.Infrastructure.Settings;

namespace TorvaldsReminder.Infrastructure.Seed;

public class DatabaseSeeder(IMongoClient mongo, MongoSettings settings)
{
    private readonly IMongoCollection<Client>  _clients  = mongo.GetDatabase(settings.DatabaseName).GetCollection<Client>("clients");
    private readonly IMongoCollection<Invoice> _invoices = mongo.GetDatabase(settings.DatabaseName).GetCollection<Invoice>("invoices");

    public async Task SeedAsync()
    {
        // Check for not inserting when there is already data
        if (await _clients.CountDocumentsAsync(FilterDefinition<Client>.Empty) > 0) return;

        var linus    = new Client { Name = "Linus Torvalds",  Email = "linus@kernel.org"   };
        var catalina = new Client { Name = "Catalina Cepeda", Email = "catalina.cepeda@monolegal.co"        };
        var jonathan  = new Client { Name = "Jonathan Ariza",  Email = "jonathan.ariza@nadeshiko.co" };

        await _clients.InsertManyAsync([linus, catalina, jonathan]);

        await _invoices.InsertManyAsync([
            new Invoice { ClientId = linus.Id,    Number = "F-2026-001", Amount = 1_500_000, Status = InvoiceStatus.PrimerRecordatorio,  DueDate = new(2026, 1, 15) },
            new Invoice { ClientId = catalina.Id, Number = "F-2026-003", Amount = 2_200_000, Status = InvoiceStatus.PrimerRecordatorio,  DueDate = new(2026, 1, 20) },
            new Invoice { ClientId = catalina.Id, Number = "F-2026-004", Amount =   950_000, Status = InvoiceStatus.SegundoRecordatorio, DueDate = new(2026, 1,  5) },
            new Invoice { ClientId = jonathan.Id,  Number = "F-2026-005", Amount = 3_100_000, Status = InvoiceStatus.SegundoRecordatorio, DueDate = new(2026, 1,  8) },
            new Invoice { ClientId = jonathan.Id,  Number = "F-2026-006", Amount =   500_000, Status = InvoiceStatus.Desactivado,         DueDate = new(2025, 12, 1) },
        ]);
    }

    public async Task ClearAndSeedAsync()
    {
        await _clients.DeleteManyAsync(FilterDefinition<Client>.Empty);
        await _invoices.DeleteManyAsync(FilterDefinition<Invoice>.Empty);
        await SeedAsync();
    }
}
