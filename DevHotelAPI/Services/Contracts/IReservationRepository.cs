using DevHotelAPI.Entities;

namespace DevHotelAPI.Services.Contracts
{
    public interface IReservationRepository
    {
        Task AddReservationAsync(Reservation reservation);
        Task<bool> CheckIfRoomIsAvailableAsync(Reservation reservation);
        Task DeleteReservationAsync(Guid id);
        Task<IEnumerable<Reservation?>> GetAllReservationsAsync();
        Task<Reservation?> GetReservationByIdAsync(Guid id);
        Task<IEnumerable<Reservation?>> GetReservationsByClientIdAsync(Guid clientId);
        Task<bool> ReservationExistsAsync(Guid id);
        Task UpdateReservationAsync(Reservation reservation);
    }
}
