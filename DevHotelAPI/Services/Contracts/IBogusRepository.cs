using Bogus;
using DevHotelAPI.Entities;
using DevHotelAPI.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace DevHotelAPI.Services.Contracts
{
    public interface IBogusRepository
    {

        List<Customer> GenerateCustomers();
        List<Reservation> GenerateReservations();
        List<Room> GenerateRooms();
        List<RoomType> GenerateRoomTypes();
        List<IdentityUser<Guid>> GenerateUsers();
        List<IdentityRole<Guid>> GenerateRoles();
        List<IdentityUserRole<Guid>> AssignRolesToFakeUsers();
    }
}
