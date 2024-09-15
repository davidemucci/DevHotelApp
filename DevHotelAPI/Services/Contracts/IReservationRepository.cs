using DevHotelAPI.Entities;
using Microsoft.AspNetCore.Identity;

namespace DevHotelAPI.Services.Contracts
{
    public interface IReservationRepository
    {
        Task AddReservationAsync(Reservation reservation, string userName);
        Task<bool> CheckIfRoomIsAvailableAsync(Reservation reservation);
        Task DeleteReservationAsync(Guid id, string userName);
        Task<IEnumerable<Reservation?>> GetAllReservationsAsync();
        Task<Reservation?> GetReservationByIdAsync(Guid id, string userName);
        Task<IEnumerable<Reservation?>> GetReservationsByCustomerIdAsync(Guid customerId, string userName);
        Task<bool> ReservationExistsAsync(Guid id);
        Task UpdateReservationAsync(Reservation reservation, string username);
    }
}
