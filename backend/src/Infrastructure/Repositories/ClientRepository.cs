using MongoDB.Driver;
using TorvaldsReminder.Application.Interfaces;
using TorvaldsReminder.Domain.Entities;
using TorvaldsReminder.Infrastructure.Settings;

namespace TorvaldsReminder.Infrastructure.Repositories;

public class ClientRepository(IMongoClient mongo, MongoSettings settings) : IClientRepository
{
    private readonly IMongoCollection<Client> _col =
        mongo.GetDatabase(settings.DatabaseName).GetCollection<Client>("clients");

    public async Task<Client?> GetByIdAsync(string id) =>
        await _col.Find(c => c.Id == id).FirstOrDefaultAsync();

    public async Task<List<Client>> GetAllAsync() =>
        await _col.Find(_ => true).ToListAsync();
}
