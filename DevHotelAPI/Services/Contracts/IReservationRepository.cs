using DevHotelAPI.Entities;
using Microsoft.AspNetCore.Identity;

namespace DevHotelAPI.Services.Contracts
{
    public interface IReservationRepository
    {
        Task AddReservationAsync(Reservation reservation);
        Task<bool> CheckIfRoomIsAvailableAsync(Reservation reservation);
        Task DeleteReservationAsync(Guid id, Guid identityUserId);
        Task<IEnumerable<Reservation?>> GetAllReservationsAsync();
        Task<Reservation?> GetReservationByIdAsync(Guid id, IdentityUser<Guid> user);
        Task<IEnumerable<Reservation?>> GetReservationsByCustomerIdAsync(Guid customerId);
        Task<bool> ReservationExistsAsync(Guid id);
        Task UpdateReservationAsync(Reservation reservation);
    }
}
