using DevHotelAPI.Contexts;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;

namespace DevHotelAPI.Services.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly HotelDevContext _context;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        public CustomerRepository(HotelDevContext context, UserManager<IdentityUser<Guid>> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task AddCustomerAsync(Customer client)
        {
            await _context.Customers.AddAsync(client);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CustomerExistsAsync(Guid id)
        {
            return await _context.Customers.AnyAsync(e => e.Id == id);
        }

        public async Task DeleteCustomerAsync(Guid id, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName) ?? throw new UnauthorizedAccessException($"User with username {userName} not found");

            var isAdmin = await _userManager.IsInRoleAsync(user, HotelDevUtilities.Roles.ADMINISTRATOR.ToString());

            var customer = await _context.Customers.FindAsync(id) ?? throw new ArgumentNullException($"Customer with id {id} not found");

            if (isAdmin || user.Id.Equals(customer.IdentityUserId))
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
            else
                throw new UnauthorizedAccessException($"User with username {userName} doesn't have the authorization to remove customer with id {customer.Id}");
        }

        public async Task<IEnumerable<Customer?>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(Guid id, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName) ?? throw new UnauthorizedAccessException($"User with username {userName} not found");
            var isAdmin = await _userManager.IsInRoleAsync(user, HotelDevUtilities.Roles.ADMINISTRATOR.ToString());
            var customer = await _context.Customers.FindAsync(id) ?? throw new ArgumentNullException($"Customer with id {id} not found");

            return isAdmin || user.Id.Equals(customer.IdentityUserId) 
                ? customer 
                : throw new UnauthorizedAccessException($"User with username {userName} doesn't have the authorization to get customer info with id {customer.Id}");
        }

        public async Task<Customer?> GetCustomerByIdAsync(Guid id)
        {
            return await _context.Customers.FindAsync(id);
        }
        public async Task UpdateCustomerAsync(Customer customer, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName) ?? throw new UnauthorizedAccessException($"User with username {userName} not found");
            var isAdmin = await _userManager.IsInRoleAsync(user, HotelDevUtilities.Roles.ADMINISTRATOR.ToString());

            customer.IdentityUserId = customer.IdentityUserId.Equals(Guid.Empty) ? user.Id : customer.IdentityUserId;

            if (isAdmin || user.Id.Equals(customer.IdentityUserId))
            {
                _context.Entry(customer).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }else
                throw new UnauthorizedAccessException($"User with username {userName} doesn't have the authorization to update customer info with id {customer.Id}");
        }
    }

}
