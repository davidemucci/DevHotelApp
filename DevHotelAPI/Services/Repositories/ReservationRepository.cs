using DevHotelAPI.Contexts;
using DevHotelAPI.Contexts.Identity;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevHotelAPI.Services.Repositories
{

    public class ReservationRepository : IReservationRepository
    {
        private readonly HotelDevContext _context;
        private readonly IdentityContext _identityContext;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        public ReservationRepository(HotelDevContext context, IdentityContext identityContext, UserManager<IdentityUser<Guid>> userManager)
        {
            _context = context;
            _identityContext = identityContext;
            _userManager = userManager;
        }

        public async Task AddReservationAsync(Reservation reservation, string userName)
        {   
            var user = await _userManager.FindByNameAsync(userName);
            if(user == null)
                throw new NullReferenceException($"User with username '{userName}' not found."); 

            if(
                await _context.Customers.Where(c => c.IdentityUserId.Equals(user.Id) && c.Id.Equals(reservation.CustomerId)).AnyAsync() || 
                await _userManager.IsInRoleAsync(user, HotelDevUtilities.Roles.ADMINISTRATOR.ToString()))
            {
                await _context.Reservations.AddAsync(reservation);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("User is not authorized to make a reservation.");
            } 

        }

        public async Task<bool> CheckIfRoomIsAvailableAsync(Reservation reservation)
        {
            return !await _context.Reservations.AnyAsync(r =>
                r.RoomNumber == reservation.RoomNumber &&
                (reservation.From < r.To && reservation.To > r.From)
            );
        }

        public async Task DeleteReservationAsync(Guid id,string userName)
        {
            var identityUser = await _userManager.FindByNameAsync(userName);
            if (identityUser == null)
                throw new NullReferenceException("User not found");

            var isAdmin = await _userManager.IsInRoleAsync(identityUser, HotelDevUtilities.Roles.ADMINISTRATOR.ToString());

            var reservationQuery = from res in _context.Reservations
                                   join user in _context.Customers on res.CustomerId equals user.Id into u2
                                   from u3 in u2
                                   where res.Id == id && (u3.IdentityUserId == identityUser.Id || isAdmin)
                                   select res;

            var reservation = await reservationQuery.FirstOrDefaultAsync();
            if (reservation is null)
                throw new Exception("No reservation was found linked to your account. Please ensure that the reservation ID is correct and that it is associated with your account.");

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Reservation?>> GetAllReservationsAsync()
        {
            return await _context.Reservations.ToListAsync();
        }

        public async Task<Reservation?> GetReservationByIdAsync(Guid id,string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new Exception("Not user found");

            var isAdmin = await _userManager.IsInRoleAsync(user, HotelDevUtilities.Roles.ADMINISTRATOR.ToString());
            IQueryable<Reservation> reseservationQuery;

            if (isAdmin)
                reseservationQuery = _context.Reservations.Where(r => r.Id.Equals(id));
            else
                reseservationQuery = from res in _context.Reservations
                                     join cust in _context.Customers on res.CustomerId equals cust.Id into u2
                                     from u3 in u2
                                     where res.Id == id && u3.IdentityUserId == user.Id
                                     select res;

            return await reseservationQuery.FirstOrDefaultAsync();
        }
        
        public async Task<IEnumerable<Reservation?>> GetReservationsByCustomerIdAsync(Guid clientId, string userName)
        {
            var identityUser = await _userManager.FindByNameAsync(userName);
            if (identityUser == null)
                throw new Exception("User not found");

            var isAdmin = await _userManager.IsInRoleAsync(identityUser, HotelDevUtilities.Roles.ADMINISTRATOR.ToString());

            IQueryable<Reservation> reservationQuery = from res in _context.Reservations
                                                       join cus in _context.Customers on res.CustomerId equals cus.Id into u2   
                                                       from cus3 in u2
                                                       where isAdmin || identityUser.Id == cus3.IdentityUserId
                                                       select res;

            
            return await reservationQuery.ToListAsync();
        }

        public async Task<bool> ReservationExistsAsync(Guid id)
        {
            return await _context.Reservations.AnyAsync(e => e.Id == id);
        }

        public async Task UpdateReservationAsync(Reservation reservation, string userName)
        {
            var identityUser = await _userManager.FindByNameAsync(userName);
          
            if (identityUser == null)
                throw new Exception("User not found");

            var user = _context.Customers.Where(c => c.IdentityUserId.Equals(identityUser.Id)).FirstOrDefault();
            var isAdmin = await _userManager.IsInRoleAsync(identityUser, HotelDevUtilities.Roles.ADMINISTRATOR.ToString());
            if (isAdmin || user.Id == reservation.CustomerId)
            {
                _context.Entry(reservation).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
        }
    }
}


