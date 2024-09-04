using Bogus;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;

namespace DevHotelAPI.Services
{
    public class BogusRepository : IBogusRepository
    {
        public readonly Guid idClient = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public readonly Guid idReservation = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private int totalRooms {  get; set; }
        private static T SeedRow<T>(Faker<T> faker, int rowId) where T : class
        {
            var record = faker.UseSeed(rowId).Generate();
            return record;
        }
       
        public List<RoomType> GenerateRoomTypes()
        {
            var id = 1;
            var count = 0;
            var descRoomTypes = new List<string>() { "Room", "TwinRoom", "Triple", "Suite" };

            var SeededOrder = new Random(12345);

            var roomTypes = new Faker<RoomType>()
                .RuleFor(r => r.Id, f => id++)
                .RuleFor(r => r.Description, f => descRoomTypes[count++])
                .RuleFor(r => r.TotalNumber, f => f.Random.Int(1, 50));


            return Enumerable.Range(1, descRoomTypes.Count)
                .Select(i => SeedRow(roomTypes, i))
                .ToList();
        }
        public List<Room> GenerateRooms(int roomTypesTotalNumber = 4, int totalRoomsBumber = 10)
        {
            var roomNumber = 100;
            totalRooms = totalRoomsBumber;
            

            var room = new Faker<Room>()
                  .RuleFor(r => r.Number, f => roomNumber++)
                  .RuleFor(r => r.RoomTypeId, f => f.Random.Int(1, roomTypesTotalNumber));

            List<Room> roomsFaker = Enumerable.Range(1, totalRooms)
                .Select(i => SeedRow(room, i))
                .ToList();
            return roomsFaker;
        }
        public List<Client> GenerateClients()
        {
            var id = 1;
            var client = new Faker<Client>()
                .RuleFor(r => r.Id, f => Guid.Parse("22222222-2222-2222-2222-22222222222" + id++.ToString()))
                .RuleFor(r => r.Email, f => f.Internet.Email())
                .RuleFor(r => r.Password, f => f.Internet.Password())
                .RuleFor(r => r.Address, f => f.Address.StreetAddress()
                );

            var clientsFaker = Enumerable.Range(1, 2)
                .Select(i => SeedRow(client, i))
                .ToList();

            return clientsFaker;    
        }
        public List<Reservation> GenerateReservations(List<Client> clientsFaker)
        {
            var id = 1;
            var total = clientsFaker.Count -1;
            var reservations = new Faker<Reservation>()
                .RuleFor(r => r.Id, f => Guid.Parse("11111111-1111-1111-1111-11111111111" + id++.ToString()))
                .RuleFor(r => r.RoomNumber, f => f.Random.Int(100, 100 + totalRooms))
                .RuleFor(r => r.ClientId, i => clientsFaker[i.Random.Int(0, total)]?.Id)
                .RuleFor(r => r.From, f => f.Date.Future(refDate: new DateTime(2024, 1, 16, 15, 15, 0)))
                .RuleFor(r => r.From, f => f.Date.Future(refDate: new DateTime(2024, 1, 18, 15, 15, 0)))
                ;

            // var reservationsFaker = reservations.Generate(5);
            var reservationsFaker = Enumerable.Range(1, 5)
                .Select(i => SeedRow(reservations, i))
                .ToList();
            return reservationsFaker;
        }
    }
}
