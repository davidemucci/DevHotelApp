using DevHotelAPI.Contexts;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DevHotelAPI.Services.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly HotelDevContext _context;

        public CustomerRepository(HotelDevContext context)
        {
            _context = context;
        }

        public async Task AddCuatomerAsync(Customer client)
        {
            await _context.Customers.AddAsync(client);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CustomerExistsAsync(Guid id)
        {
            return await _context.Customers.AnyAsync(e => e.Id == id);
        }

        public async Task DeleteCustomerAsync(Guid id)
        {
            var client = await _context.Customers.FindAsync(id);
            if (client != null)
            {
                _context.Customers.Remove(client);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Customer?>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(Guid id)
        {
            return await _context.Customers.FindAsync(id);
        }
        public async Task UpdateCustomerAsync(Customer client)
        {
            _context.Entry(client).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }

}
