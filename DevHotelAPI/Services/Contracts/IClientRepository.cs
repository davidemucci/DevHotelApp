using DevHotelAPI.Entities;

namespace DevHotelAPI.Services.Contracts
{
    public interface IClientRepository
    {
        Task AddClientAsync(Client client);
        Task<bool> ClientExistsAsync(Guid id);
        Task DeleteClientAsync(Guid id);
        Task<IEnumerable<Client?>> GetAllClientsAsync();
        Task<Client?> GetClientByIdAsync(Guid id);
        Task UpdateClientAsync(Client client);
    }
}
