using Bogus;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;

namespace DevHotelAPI.Services
{
    public class BogusRepository : IBogusRepository
    {
        public readonly Guid idCustomer = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public readonly Guid idReservation = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private int totalRooms {  get; set; }
        public List<Customer> GenerateCustomers()
        {
            var id = 1;
            var customer = new Faker<Customer>()
                .RuleFor(r => r.Id, f => Guid.Parse("22222222-2222-2222-2222-22222222222" + id++.ToString()))
                .RuleFor(r => r.Email, f => f.Internet.Email())
                .RuleFor(r => r.ProfileId, f => Guid.Parse("99999999-9999-9999-9999-99999999999" + id++.ToString()))
                .RuleFor(r => r.Address, f => f.Address.StreetAddress()
                );

            var customersFaker = Enumerable.Range(1, 2)
                .Select(i => SeedRow(customer, i))
                .ToList();

            return customersFaker;
        }

        public List<Reservation> GenerateReservations(List<Customer> customersFaker)
        {
            var id = 1;
            var total = customersFaker.Count - 1;
            var roomNumberStart = 100;
            var reservations = new Faker<Reservation>()
                .RuleFor(r => r.Id, f => Guid.Parse("11111111-1111-1111-1111-11111111111" + id++.ToString()))
                .RuleFor(r => r.RoomNumber, f => roomNumberStart++)
                .RuleFor(r => r.CustomerId, i => customersFaker[i.Random.Int(0, total)]?.Id)
                .RuleFor(r => r.From, f => new DateTime(2027, 1, 16, 15, 15, 0))
                .RuleFor(r => r.To, f => new DateTime(2027, 1, 18, 15, 15, 0))
                ;

            // var reservationsFaker = reservations.Generate(5);
            var reservationsFaker = Enumerable.Range(1, 5)
                .Select(i => SeedRow(reservations, i))
                .ToList();
            return reservationsFaker;
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

        public List<RoomType> GenerateRoomTypes()
        {
            var id = 1;
            var count = 0;
            var descRoomTypes = new List<string>() { "Room", "TwinRoom", "Triple", "Suite" };
            var capacity = 1;

            var SeededOrder = new Random(12345);

            var roomTypes = new Faker<RoomType>()
                .RuleFor(r => r.Id, f => id++)
                .RuleFor(r => r.Capacity, f => capacity++)
                .RuleFor(r => r.Description, f => descRoomTypes[count++])
                .RuleFor(r => r.TotalNumber, f => f.Random.Int(1, 50));


            return Enumerable.Range(1, descRoomTypes.Count)
                .Select(i => SeedRow(roomTypes, i))
                .ToList();
        }

        private static T SeedRow<T>(Faker<T> faker, int rowId) where T : class
        {
            var record = faker.UseSeed(rowId).Generate();
            return record;
        }
    }
}
