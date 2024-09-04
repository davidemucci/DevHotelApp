using DevHotelAPI.Entities;

namespace DevHotelAPI.Services.Contracts
{
    public interface IRoomTypeRepository
    {
        Task<IEnumerable<RoomType>> GetAllRoomTypesAsync();
        Task<RoomType?> GetRoomTypeByIdAsync(int id);
        Task AddRoomTypeAsync(RoomType roomType);
        Task UpdateRoomTypeAsync(RoomType roomType);
        Task DeleteRoomTypeAsync(int id);
    }
}
