using DevHotelAPI.Entities;

namespace DevHotelAPI.Services.Contracts
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation?>> GetAllReservationsAsync();
        Task<IEnumerable<Reservation?>> GetReservationsByClientIdAsync(Guid clientId);
        Task<Reservation?> GetReservationByIdAsync(Guid id);
        Task AddReservationAsync(Reservation reservation);
        Task UpdateReservationAsync(Reservation reservation);
        Task DeleteReservationAsync(Guid id);
        Task<bool> ReservationExistsAsync(Guid id);
    }
}
