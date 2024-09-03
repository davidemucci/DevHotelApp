using DevHotelAPI.Contexts;
using DevHotelAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevHotelAPI.Services.Repositories
{
    public class RoomTypeRepository : IRoomTypeRepository
    {
        private readonly HotelDevContext _context;

        public RoomTypeRepository(HotelDevContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomType>> GetAllRoomTypesAsync()
        {
            return await _context.RoomTypes.AsNoTracking().ToListAsync();
        }

        public async Task<RoomType?> GetRoomTypeByIdAsync(int id)
        {
            return await _context.RoomTypes.Where(r => r.Id.Equals(id)).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task AddRoomTypeAsync(RoomType roomType)
        {
            await _context.RoomTypes.AddAsync(roomType);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRoomTypeAsync(RoomType roomType)
        {
            _context.Entry(roomType).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoomTypeAsync(int id)
        {
            var roomType = await _context.RoomTypes.FindAsync(id);
            if (roomType != null)
            {
                _context.RoomTypes.Remove(roomType);
                await _context.SaveChangesAsync();
            }
        }
    }

}
