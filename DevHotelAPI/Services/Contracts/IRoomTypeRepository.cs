using DevHotelAPI.Entities;

namespace DevHotelAPI.Services.Contracts
{
    public interface IRoomTypeRepository
    {
        Task AddRoomTypeAsync(RoomType roomType);
        Task DeleteRoomTypeAsync(int id);
        Task<IEnumerable<RoomType>> GetAllRoomTypesAsync();
        Task<RoomType?> GetRoomTypeByIdAsync(int id);
        Task UpdateRoomTypeAsync(RoomType roomType);
    }
}
