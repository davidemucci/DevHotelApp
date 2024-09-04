using Bogus;
using DevHotelAPI.Entities;

namespace DevHotelAPI.Services.Contracts
{
    public interface IBogusRepository
    {

        public List<RoomType> GenerateRoomTypes();
        List<Room> GenerateRooms(int roomTypesTotalNumber = 4, int totalRoomsBumber = 10);
        List<Client> GenerateClients();
        List<Reservation> GenerateReservations(List<Client> clientsFaker);
    }
}
