using DevHotelAPI.Entities;

namespace DevHotelAPI.Services.Contracts
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room?>> GetAllRoomAsync();
        Task<Room?> GetByIdRoomAsync(int id);
        Task AddRoomAsync(Room roomType);
        Task UpdateRoomAsync(Room roomType);
        Task DeleteRoomAsync(int id);

    }
}
