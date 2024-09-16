using DevHotelAPI.Entities;

namespace DevHotelAPI.Services.Contracts
{
    public interface ICustomerRepository
    {
        Task AddCustomerAsync(Customer customer);
        Task<bool> CustomerExistsAsync(Guid id);
        Task DeleteCustomerAsync(Guid id);
        Task<IEnumerable<Customer?>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(Guid id);
        Task UpdateCustomerAsync(Customer customer);
    }
}
