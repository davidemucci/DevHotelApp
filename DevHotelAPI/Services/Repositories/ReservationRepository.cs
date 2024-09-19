using DevHotelAPI.Contexts;
using DevHotelAPI.Contexts.Identity;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using DevHotelAPI.Services.Utility;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevHotelAPI.Services.Repositories
{

    public class ReservationRepository(HotelDevContext context, IdentityContext identityContext, UserManager<IdentityUser<Guid>> userManager) : IReservationRepository
    {
        private readonly HotelDevContext _context = context;
        private readonly IdentityContext _identityContext = identityContext;
        private readonly UserManager<IdentityUser<Guid>> _userManager = userManager;

        public async Task AddReservationAsync(Reservation reservation, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName) ?? throw new UnauthorizedAccessException($"User not found with username {userName}");
            var isAdmin = await _userManager.IsInRoleAsync(user, HotelDevUtilities.Roles.ADMINISTRATOR.ToString());

            if
            (
                await _context.Customers.Where(c => c.IdentityUserId.Equals(user.Id) && c.Id.Equals(reservation.CustomerId)).AnyAsync() ||
                isAdmin
            )
            {
                await _context.Reservations.AddAsync(reservation);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new UnauthorizedAccessException($"User {userName} is not authorized to make a reservation.");
            }

        }

        public async Task<bool> CheckIfRoomIsAvailableAsync(Reservation reservation)
        {
            return !await _context.Reservations.AnyAsync(r =>
                r.RoomNumber == reservation.RoomNumber &&
                (reservation.From < r.To && reservation.To > r.From)
            );
        }

        public async Task DeleteReservationAsync(Guid id, string userName)
        {
            var identityUser = await _userManager.FindByNameAsync(userName) ?? 
                throw new UnauthorizedAccessException($"User not found with username {userName}");

            var isAdmin = await _userManager.IsInRoleAsync(identityUser, HotelDevUtilities.Roles.ADMINISTRATOR.ToString());

            var reservationQuery = from res in _context.Reservations
                                   join user in _context.Customers on res.CustomerId equals user.Id into u2
                                   from u3 in u2
                                   where res.Id == id && (u3.IdentityUserId == identityUser.Id || isAdmin)
                                   select res;

            var reservation = await reservationQuery.FirstOrDefaultAsync() ??
                throw new ArgumentNullException("No reservation was found linked to your account. Please ensure that the reservation ID is correct and that it is associated with your account.");

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Reservation?>> GetAllReservationsAsync()
        {
            return await _context.Reservations.ToListAsync();
        }

        public async Task<Reservation?> GetReservationByIdAsync(Guid id, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName) 
                ?? throw new UnauthorizedAccessException($"User not found with username {userName}");

            var isAdmin = await _userManager.IsInRoleAsync(user, HotelDevUtilities.Roles.ADMINISTRATOR.ToString());
            IQueryable<Reservation> reseservationQuery;

            reseservationQuery = from res in _context.Reservations
                                 join cust in _context.Customers on res.CustomerId equals cust.Id into u2
                                 from u3 in u2
                                 where res.Id == id && (u3.IdentityUserId == user.Id || isAdmin)
                                 select res;

            return await reseservationQuery.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Reservation?>> GetReservationsByCustomerIdAsync(Guid clientId, string userName)
        {
            var identityUser = await _userManager.FindByNameAsync(userName) ?? throw new UnauthorizedAccessException($"User not found with username {userName}");

            var isAdmin = await _userManager.IsInRoleAsync(identityUser, HotelDevUtilities.Roles.ADMINISTRATOR.ToString());

            IQueryable<Reservation> reservationQuery = from res in _context.Reservations
                                                       join cus in _context.Customers on res.CustomerId equals cus.Id into u2
                                                       from cus3 in u2
                                                       where res.CustomerId == clientId && (isAdmin || identityUser.Id == cus3.IdentityUserId)
                                                       select res;


            return await reservationQuery.ToListAsync();
        }

        public async Task<bool> ReservationExistsAsync(Guid id)
        {
            return await _context.Reservations.AnyAsync(e => e.Id == id);
        }

        public async Task UpdateReservationAsync(Reservation reservation, string userName)
        {
            var identityUser = await _userManager.FindByNameAsync(userName) ?? throw new UnauthorizedAccessException($"User not found with username {userName}");

            var isItsOwnReservation = await _context.Customers.Where(c => 
                c.IdentityUserId.Equals(identityUser.Id) && 
                reservation.CustomerId.Equals(c.Id)).AnyAsync();

            var isAdmin = await _userManager.IsInRoleAsync(
               identityUser, HotelDevUtilities.Roles.ADMINISTRATOR.ToString());

            if (isAdmin || isItsOwnReservation)
            {
                _context.Entry(reservation).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            else
                throw new UnauthorizedAccessException($"Customer not found with IdentityUserId {identityUser.Id}");
        }
    }
}


