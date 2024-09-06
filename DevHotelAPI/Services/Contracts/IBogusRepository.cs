using Bogus;
using DevHotelAPI.Entities;

namespace DevHotelAPI.Services.Contracts
{
    public interface IBogusRepository
    {

        List<Client> GenerateClients();
        List<Reservation> GenerateReservations(List<Client> clientsFaker);
        List<Room> GenerateRooms(int roomTypesTotalNumber = 4, int totalRoomsBumber = 10);
        public List<RoomType> GenerateRoomTypes();
    }
}
