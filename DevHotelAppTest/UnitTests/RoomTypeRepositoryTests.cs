using Bogus;
using DevHotelAPI.Contexts;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevHotelAppTest.UnitTests
{
    public class RoomTypeRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly HotelDevContext _context;
        private readonly RoomTypeRepository _repository;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        public RoomTypeRepositoryTests(DatabaseFixture databaseFixture)
        {
            _context = databaseFixture._context;
            _userManager = databaseFixture._userManager;
            _repository = new RoomTypeRepository(_context);
        }

        [Fact]
        public async Task AddRoomTypeAsync_ShouldAddRoomType()
        {
            // Arrange
            var roomType = new Faker<RoomType>()
                .RuleFor(r => r.Id, f => f.Random.Int(1, 1000))
                .RuleFor(r => r.Capacity, f => f.Random.Int(1, 5))
                .RuleFor(r => r.Description, f => f.Lorem.Sentence())
                .RuleFor(r => r.TotalNumber, f => f.Random.Int(1, 100))
                .Generate();

            // Act
            await _repository.AddRoomTypeAsync(roomType);

            // Assert
            var addedRoomType = await _context.RoomTypes.FindAsync(roomType.Id);
            Assert.NotNull(addedRoomType);
            Assert.Equal(roomType.Id, addedRoomType.Id);
        }

        [Fact]
        public async Task DeleteRoomTypeAsync_ShouldRemoveRoomType()
        {
            // Arrange
            var roomType = new Faker<RoomType>()
                .RuleFor(r => r.Id, f => f.Random.Int(1, 1000))
                .RuleFor(r => r.Capacity, f => f.Random.Int(1, 5))
                .RuleFor(r => r.Description, f => f.Lorem.Sentence())
                .RuleFor(r => r.TotalNumber, f => f.Random.Int(1, 100))
                .Generate();

            await _context.RoomTypes.AddAsync(roomType);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteRoomTypeAsync(roomType.Id);

            // Assert
            var deletedRoomType = await _context.RoomTypes.FindAsync(roomType.Id);
            Assert.Null(deletedRoomType);
        }

        [Fact]
        public async Task GetAllRoomTypesAsync_ShouldReturnAllRoomTypes()
        {
            // Arrange
            var roomTypes = new Faker<RoomType>()
                .RuleFor(r => r.Id, f => f.Random.Int(1, 1000))
                .RuleFor(r => r.Capacity, f => f.Random.Int(1, 5))
                .RuleFor(r => r.Description, f => f.Lorem.Sentence())
                .RuleFor(r => r.TotalNumber, f => f.Random.Int(1, 100))
                .Generate(5);

            var roomsType =  _context.RoomTypes.Count();

            await _context.RoomTypes.AddRangeAsync(roomTypes);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllRoomTypesAsync();

            // Assert
            Assert.Equal(5 + roomsType, result.Count());
        }

        [Fact]
        public async Task GetRoomTypeByIdAsync_ShouldReturnRoomType()
        {
            // Arrange
            var roomType = new Faker<RoomType>()
                .RuleFor(r => r.Id, f => f.Random.Int(1, 1000))
                .RuleFor(r => r.Capacity, f => f.Random.Int(1, 5))
                .RuleFor(r => r.Description, f => f.Lorem.Sentence())
                .RuleFor(r => r.TotalNumber, f => f.Random.Int(1, 100))
                .Generate();

            await _context.RoomTypes.AddAsync(roomType);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetRoomTypeByIdAsync(roomType.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(roomType.Id, result.Id);
        }

        [Fact]
        public async Task UpdateRoomTypeAsync_ShouldUpdateRoomType()
        {
            // Arrange
            var roomType = new Faker<RoomType>()
                .RuleFor(r => r.Id, f => f.Random.Int(1, 1000))
                .RuleFor(r => r.Capacity, f => f.Random.Int(1, 5))
                .RuleFor(r => r.Description, f => f.Lorem.Sentence())
                .RuleFor(r => r.TotalNumber, f => f.Random.Int(1, 100))
                .Generate();

            await _context.RoomTypes.AddAsync(roomType);
            await _context.SaveChangesAsync();

            roomType.Description = "Updated Description";

            // Act
            await _repository.UpdateRoomTypeAsync(roomType);

            // Assert
            var updatedRoomType = await _context.RoomTypes.FindAsync(roomType.Id);
            Assert.NotNull(updatedRoomType);
            Assert.Equal("Updated Description", updatedRoomType.Description);
        }

        [Fact]
        public async Task RoomExistsAsync_ShouldReturnTrueIfRoomTypeExists()
        {
            // Arrange
            var roomType = new Faker<RoomType>()
                .RuleFor(r => r.Id, f => f.Random.Int(1, 1000))
                .RuleFor(r => r.Capacity, f => f.Random.Int(1, 5))
                .RuleFor(r => r.Description, f => f.Lorem.Sentence())
                .RuleFor(r => r.TotalNumber, f => f.Random.Int(1, 100))
                .Generate();

            await _context.RoomTypes.AddAsync(roomType);
            await _context.SaveChangesAsync();

            // Act
            var exists = await _repository.RoomExistsAsync(roomType.Id);

            // Assert
            Assert.True(exists);
        }
    }
}
