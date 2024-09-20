using Bogus;
using DevHotelAPI.Entities;
using DevHotelAPI.Entities.Identity;
using DevHotelAPI.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace DevHotelAPI.Services.Repositories
{
    public class BogusRepository : IBogusRepository
    {
        public Guid AdminId { get; private set; } = Guid.Parse("22222222-2222-2222-2222-222222222221");
        public Guid AdminIdenityId { get; private set; } = Guid.Parse("99999999-9999-9999-9999-999999999991");
        public Guid ConsumerId { get; private set; } = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public Guid ConsumerIdentityId { get; private set; } = Guid.Parse("99999999-9999-9999-9999-999999999992");
        public int CustomersCount { get; private set; } = 2;
        public List<string> DescriptionsRoomTypes { get; private set; } = ["Room", "TwinRoom", "Triple", "Suite"];
        /// Only Last Reservation has CostumerId as AdminId, others ConsumerId
        public List<Guid> ReservationsId { get; private set; } = [
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Guid.Parse("11111111-1111-1111-1111-111111111112"),
            Guid.Parse("11111111-1111-1111-1111-111111111113"),
            Guid.Parse("11111111-1111-1111-1111-111111111114"),
            Guid.Parse("11111111-1111-1111-1111-111111111115") ];

        public Dictionary<string, Guid> Roles = new()
        {
            { "Administrator" , Guid.Parse("19999999-9999-9999-9999-999999999991") },
            { "Consumer" , Guid.Parse("19999999-9999-9999-9999-999999999992") },
        };
        public List<int> RoomsId { get; private set; } = [100, 101, 102, 103, 104, 105, 106, 107, 108, 109];
        public List<int> RoomTypesId { get; private set; } = [1, 2, 3, 4];
        public string UserNameAdmin { get; private set; } = "ADMIN";
        public string UserNameConsumer { get; private set; } = "CONSUMER";

        public List<IdentityUserRole<Guid>> AssignRolesToFakeUsers()
        {
            var id = 0;
            var userRoleFaker = new Faker<IdentityUserRole<Guid>>()
                .RuleFor(r => r.RoleId, f => id == 0 ? Roles["Administrator"] : Roles["Consumer"])
                .RuleFor(r => r.UserId, f =>
                {
                    var userId = id == 0 ? AdminIdenityId : ConsumerIdentityId;
                    id++;
                    return userId;
                });

            var usersRolesFaker = Enumerable.Range(1, Roles.Count)
                .Select(i => SeedRow(userRoleFaker, i))
                .ToList();

            return usersRolesFaker;

        }

        public List<Customer> GenerateCustomers()
        {
            var id = 0;
            var customer = new Faker<Customer>()
                .RuleFor(r => r.Id, f =>
                {
                    id++;
                    return id == 1 ? AdminId : ConsumerId;
                })
                .RuleFor(r => r.Email, f => f.Internet.Email())
                .RuleFor(r => r.IdentityUserId, f =>
                {
                    return id == 1 ? AdminIdenityId : ConsumerIdentityId;
                })
                .RuleFor(r => r.Address, f => f.Address.StreetAddress()
                );

            var customersFaker = Enumerable.Range(1, CustomersCount)
                .Select(i => SeedRow(customer, i))
                .ToList();

            return customersFaker;
        }

        public List<Reservation> GenerateReservations()
        {
            var id = 0;
            var reservations = new Faker<Reservation>()
                .RuleFor(r => r.Id, f =>
                {
                    return ReservationsId[id];
                })
                .RuleFor(r => r.RoomNumber, f =>
                {
                    var number = RoomsId[id];
                    id++;
                    return number;
                })
                .RuleFor(r => r.CustomerId, ConsumerId)
                .RuleFor(r => r.From, f => new DateTime(2027, 1, 16, 15, 15, 0))
                .RuleFor(r => r.To, f => new DateTime(2027, 1, 18, 15, 15, 0))
                ;

            var reservationsFaker = Enumerable.Range(1, ReservationsId.Count)
                .Select(i => SeedRow(reservations, i))
                .ToList();

            reservationsFaker.Last().CustomerId = AdminId;

            ReservationsId = reservationsFaker.Select(i => i.Id).ToList();
            return reservationsFaker;
        }

        public List<IdentityRole<Guid>> GenerateRoles()
        {
            var id = 0;

            var roleFaker = new Faker<IdentityRole<Guid>>()
            .RuleFor(u => u.Id, f => id == 0 ? Roles["Administrator"] : Roles["Consumer"])
            .RuleFor(u => u.Name, f => id == 0 ? "Administrator" : "Consumer")
            .RuleFor(u => u.NormalizedName, f =>
            {
                var name = id == 0 ? "Administrator" : "Consumer";
                id++;
                return name;
            });

            var rolesFaker = Enumerable.Range(1, Roles.Count)
                .Select(i => SeedRow(roleFaker, i))
                .ToList();

            return rolesFaker;
        }

        public List<Room> GenerateRooms()
        {
            var id = 0;
            var room = new Faker<Room>()
                  .RuleFor(r => r.Number, f => RoomsId[id++])
                  .RuleFor(r => r.RoomTypeId, f => f.Random.Int(1, DescriptionsRoomTypes.Count));

            List<Room> roomsFaker = Enumerable.Range(1, RoomsId.Count)
                .Select(i => SeedRow(room, i))
                .ToList();

            return roomsFaker;
        }

        public List<RoomType> GenerateRoomTypes()
        {
            var id = 1;
            var count = 0;
            var capacity = 1;

            var SeededOrder = new Random(12345);

            var roomTypesFaker = new Faker<RoomType>()
                .RuleFor(r => r.Id, f => id++)
                .RuleFor(r => r.Capacity, f => capacity++)
                .RuleFor(r => r.Description, f => DescriptionsRoomTypes[count++])
                .RuleFor(r => r.TotalNumber, f => f.Random.Int(1, 50));

            var roomTypes = Enumerable.Range(1, DescriptionsRoomTypes.Count)
                .Select(i => SeedRow(roomTypesFaker, i))
                .ToList();

            return roomTypes;
        }

        public List<IdentityUser<Guid>> GenerateUsers()
        {
            var nameCounter = 0;
            var emailCounter = 0;
            var passHasher = new PasswordHasher<IdentityUser<Guid>>();
            var names = new List<string>() { "admin", "consumer" };
            var email = new List<string>() { "admin@email.com", "consumer@email.com" };
            var userFaker = new Faker<IdentityUser<Guid>>()
            .RuleFor(u => u.Id, f =>
            {
                return emailCounter == 0 ? AdminIdenityId : ConsumerIdentityId;
            })
            .RuleFor(u => u.UserName, f => names[nameCounter])
            .RuleFor(u => u.NormalizedUserName, f => names[nameCounter++].ToUpper())
            .RuleFor(u => u.PasswordHash, (f, u) => passHasher.HashPassword(u, "Passw0rd!"))
            .RuleFor(u => u.Email, f => email[emailCounter])
            .RuleFor(u => u.NormalizedEmail, f => email[emailCounter++].ToUpper())
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber());

            var usersFaker = Enumerable.Range(1, CustomersCount)
                .Select(i => SeedRow(userFaker, i))
                .ToList();


            return usersFaker;
        }
        private static T SeedRow<T>(Faker<T> faker, int rowId) where T : class
        {
            var record = faker.UseSeed(rowId).Generate();
            return record;
        }
    }
}
