using Bogus;
using DevHotelAPI.Contexts;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevHotelAppTest.UnitTests
{
    public class RoomRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly HotelDevContext _context;
        private readonly RoomRepository _repository;

        public RoomRepositoryTests(DatabaseFixture databaseFixture)
        {
            databaseFixture.ResetContext();
            _context = databaseFixture.Context;
            _repository = new RoomRepository(_context); 
        }

        [Fact]
        public async Task AddRoomAsync_ShouldAddRoom()
        {
            // Arrange
            var room = new Faker<Room>()
                .RuleFor(r => r.Number, 999)
                .RuleFor(r => r.Description, f => f.Lorem.Sentence())
                .RuleFor(r => r.RoomTypeId, f => f.Random.Int(1, 5))
                .Generate();

            // Act
            await _repository.AddRoomAsync(room);

            // Assert
            var addedRoom = await _context.Rooms.FindAsync(room.Number);
            Assert.NotNull(addedRoom);
            Assert.Equal(room.Number, addedRoom.Number);
        }

        [Fact]
        public async Task DeleteRoomAsync_ShouldRemoveRoom()
        {
            // Arrange
            var room = new Faker<Room>()
                .RuleFor(r => r.Number, 998)
                .RuleFor(r => r.Description, f => f.Lorem.Sentence())
                .RuleFor(r => r.RoomTypeId, f => f.Random.Int(1, 5))
                .Generate();

            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteRoomAsync(room);

            // Assert
            var deletedRoom = await _context.Rooms.FindAsync(room.Number);
            Assert.Null(deletedRoom);
        }

        [Fact]
        public async Task GetAllRoomAsync_ShouldReturnAllRooms()
        {
            var number = 200;
            // Arrange
            var rooms = new Faker<Room>()
                .RuleFor(r => r.Number, f => number++)
                .RuleFor(r => r.Description, f => f.Lorem.Sentence())
                .RuleFor(r => r.RoomTypeId, f => f.Random.Int(1, 5))
                .Generate(5);
            var count = _context.Rooms.Count();
            await _context.Rooms.AddRangeAsync(rooms);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllRoomAsync();

            // Assert
            Assert.Equal(5 + count, result.Count());
        }

        [Fact]
        public async Task GetByIdRoomAsync_ShouldReturnRoom()
        {
            // Arrange
            var room = new Faker<Room>()
                .RuleFor(r => r.Number, 997)
                .RuleFor(r => r.Description, f => f.Lorem.Sentence())
                .RuleFor(r => r.RoomTypeId, f => f.Random.Int(1, 5))
                .Generate();

            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdRoomAsync(room.Number);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(room.Number, result.Number);
        }

        [Fact]
        public async Task UpdateRoomAsync_ShouldUpdateRoom()
        {
            // Arrange
            var room = new Faker<Room>()
                .RuleFor(r => r.Number, 996)
                .RuleFor(r => r.Description, f => f.Lorem.Sentence())
                .RuleFor(r => r.RoomTypeId, f => f.Random.Int(1, 5))
                .Generate();

            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            room.Description = "Updated Description";

            // Act
            await _repository.UpdateRoomAsync(room);

            // Assert
            var updatedRoom = await _context.Rooms.FindAsync(room.Number);
            Assert.NotNull(updatedRoom);
            Assert.Equal("Updated Description", updatedRoom.Description);
        }

        [Fact]
        public async Task GetAllRoomsAvailableAsync_ShouldReturnAvailableRooms()
        {
            // Arrange
            var fromDate = new DateTime(2029, 9, 1);
            var toDate = new DateTime(2029, 9, 10);
            var capacity = 2;

            var id = 400;

            var rooms = new Faker<Room>()
                .RuleFor(r => r.Number, f => id++)
                .RuleFor(r => r.Description, f => f.Lorem.Sentence())
                .RuleFor(r => r.RoomTypeId, f => f.Random.Int(2, 5))
                .Generate(5);

            var reservations = new Faker<Reservation>()
                .RuleFor(r => r.RoomNumber, f => rooms.Last().Number)
                .RuleFor(r => r.From, f => new DateTime(2029, 9, 5))
                .RuleFor(r => r.To, f => new DateTime(2029, 9, 7))
                .Generate(1);
            var start = 2;
            var roomTypes = new Faker<RoomType>()
                .RuleFor(rt => rt.Capacity, f => start++)
                .RuleFor(rt => rt.Description, f => f.Lorem.Sentence())
                .Generate(2);

            var count = await _repository.GetAllRoomsAvailableAsync(fromDate, toDate, capacity);

            await _context.Rooms.AddRangeAsync(rooms);
            await _context.Reservations.AddRangeAsync(reservations);
            await _context.RoomTypes.AddRangeAsync(roomTypes);
            await _context.SaveChangesAsync();

            // Act
            var roomsType = _context.RoomTypes.ToList();
            var result = await _repository.GetAllRoomsAvailableAsync(fromDate, toDate, capacity);
            var all = await _repository.GetAllRoomAsync();
            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(4 + count.Count(), result.Count());
            Assert.Contains(result, r => r.Number == rooms.ToList().First().Number);
        }
    }
}
