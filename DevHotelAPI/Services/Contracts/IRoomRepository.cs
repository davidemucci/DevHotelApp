using DevHotelAPI.Entities;

namespace DevHotelAPI.Services.Contracts
{
    public interface IRoomRepository
    {
        Task AddRoomAsync(Room roomType);
        Task DeleteRoomAsync(int id);
        Task<IEnumerable<Room?>> GetAllRoomAsync();
        Task<IEnumerable<Room>> GetAllRoomsAvailableAsync(DateTime fromDate, DateTime toDate, int capacity);
        Task<Room?> GetByIdRoomAsync(int id);
        Task UpdateRoomAsync(Room roomType);
    }
}
