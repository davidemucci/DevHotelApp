using DevHotelAPI.Contexts;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DevHotelAPI.Services.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelDevContext _context;

        public RoomRepository(HotelDevContext context)
        {
            _context = context;
        }
        public async Task AddRoomAsync(Room room)
        {
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Room?>> GetAllRoomAsync()
        {
            return await _context.Rooms.ToListAsync();
        }

        public async Task<Room?> GetByIdRoomAsync(int id)
        {
            return await _context.Rooms.FindAsync(id);
        }


        public async Task UpdateRoomAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
        }
    }

}
