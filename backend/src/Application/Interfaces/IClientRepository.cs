using TorvaldsReminder.Domain.Entities;

namespace TorvaldsReminder.Application.Interfaces;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(string id);
    Task<List<Client>> GetAllAsync();
}