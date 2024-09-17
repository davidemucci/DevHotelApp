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

        public async Task DeleteRoomAsync(Room room)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Room?>> GetAllRoomAsync()
        {
            return await _context.Rooms.ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAvailableAsync(DateTime fromDate, DateTime toDate, int capacity)
        {
            var result = from room in _context.Rooms
                         join reservation in _context.Reservations on room.Number equals reservation.RoomNumber into r2
                         from r3 in r2.DefaultIfEmpty()
                         join types in _context.RoomTypes on room.RoomTypeId equals types.Id into t2
                         from t3 in t2
                         where
                         t3.Capacity >= capacity &&
                         (r3 == null || !(fromDate < r3.To && toDate > r3.From))
                         select room;

            return await result.OrderBy(x => x.Number).ToListAsync();
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
    }

}
