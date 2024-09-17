using DevHotelAPI.Contexts;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace DevHotelAPI.Services.Repositories
{
    public class RoomTypeRepository(HotelDevContext context) : IRoomTypeRepository
    {
        private readonly HotelDevContext _context = context;

        public async Task AddRoomTypeAsync(RoomType roomType)
        {
            await _context.RoomTypes.AddAsync(roomType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoomTypeAsync(int id)
        {
            var roomType = await _context.RoomTypes.FindAsync(id) ??
                throw new ArgumentNullException($"Room Type with id {id} not found in the db.");

            _context.RoomTypes.Remove(roomType);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RoomType>> GetAllRoomTypesAsync()
        {
            return await _context.RoomTypes.AsNoTracking().ToListAsync();
        }

        public async Task<RoomType?> GetRoomTypeByIdAsync(int id)
        {
            return await _context.RoomTypes.FindAsync(id);
        }

        public async Task UpdateRoomTypeAsync(RoomType roomType)
        {
            if (roomType.Id == 0)
                throw new ArgumentNullException(null, "Invalid RoomType Id");

            _context.Entry(roomType).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RoomExistsAsync(int id)
        {
           return await _context.RoomTypes.AnyAsync(i => i.Id == id);
        }
    }

}
