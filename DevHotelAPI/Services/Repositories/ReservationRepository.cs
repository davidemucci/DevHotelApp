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

        public async Task AddReservationAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckIfRoomIsAvailableAsync(Reservation reservation)
        {
            return !await _context.Reservations.AnyAsync(r =>
                r.RoomNumber == reservation.RoomNumber &&
                (reservation.From < r.To && reservation.To > r.From)
            );
        }

        public async Task DeleteReservationAsync(Guid id, Guid identityUserId)
        {
            var reservationQuery = from res in _context.Reservations
                                   join user in _context.Customers on res.CustomerId equals user.Id into u2
                                   from u3 in u2
                                   where res.Id == id && u3.IdentityUserId == identityUserId
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

        public async Task<Reservation?> GetReservationByIdAsync(Guid id, IdentityUser<Guid> user)
        {
            var isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");
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
        public async Task<IEnumerable<Reservation?>> GetReservationsByCustomerIdAsync(Guid clientId)
        {
            return await _context.Reservations.Where(r => r.CustomerId.Equals(clientId)).ToListAsync();
        }

        public async Task<bool> ReservationExistsAsync(Guid id)
        {
            return await _context.Reservations.AnyAsync(e => e.Id == id);
        }

        public async Task UpdateReservationAsync(Reservation reservation)
        {
            _context.Entry(reservation).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}


