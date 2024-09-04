using DevHotelAPI.Entities;

namespace DevHotelAPI.Services.Contracts
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client?>> GetAllClientsAsync();
        Task<Client?> GetClientByIdAsync(Guid id);
        Task AddClientAsync(Client client);
        Task UpdateClientAsync(Client client);
        Task DeleteClientAsync(Guid id);
        Task<bool> ClientExistsAsync(Guid id);
    }
}
