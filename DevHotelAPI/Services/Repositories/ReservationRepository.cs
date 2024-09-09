using DevHotelAPI.Contexts;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace DevHotelAPI.Services.Repositories
{

    public class ReservationRepository : IReservationRepository
    {
        private readonly HotelDevContext _context;

        public ReservationRepository(HotelDevContext context)
        {
            _context = context;
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

        public async Task DeleteReservationAsync(Guid id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
                _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Reservation?>> GetAllReservationsAsync()
        {
            return await _context.Reservations.ToListAsync();
        }

        public async Task<Reservation?> GetReservationByIdAsync(Guid id)
        {
            return await _context.Reservations.FindAsync(id);
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


