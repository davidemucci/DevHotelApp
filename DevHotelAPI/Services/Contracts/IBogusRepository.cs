using Bogus;
using DevHotelAPI.Entities;

namespace DevHotelAPI.Services.Contracts
{
    public interface IBogusRepository
    {

        List<Customer> GenerateCustomers();
        List<Reservation> GenerateReservations(List<Customer> customersFaker);
        List<Room> GenerateRooms(int roomTypesTotalNumber = 4, int totalRoomsBumber = 10);
        public List<RoomType> GenerateRoomTypes();
    }
}
