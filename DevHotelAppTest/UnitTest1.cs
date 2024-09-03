using Bogus;
using DevHotelAPI.Contexts;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DevHotelAppTest
{

    public class RoomTypesControllerTests
    {
        private readonly IRoomTypeRepository _repository;
        private readonly HotelDevContext _context;
        private readonly IBogusRepository _bogusRepo;

        public RoomTypesControllerTests(IBogusRepository bogusRepo)
        {
            var options = new DbContextOptionsBuilder<HotelDevContext>()
                .UseInMemoryDatabase(databaseName: "TestHotelDevDb")
                .Options;

            _context = new HotelDevContext(options, bogusRepo);
            _repository = new RoomTypeRepository(_context);
            _bogusRepo = bogusRepo;

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var roomTypesFaker = _bogusRepo.GenerateRoomTypes();
            var roomsFaker = _bogusRepo.GenerateRooms(4, 10);
            var clientsFaker = _bogusRepo.GenerateClients();
            var reservationsFaker = _bogusRepo.GenerateReservations(clientsFaker);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetRoomTypes_ReturnsAllRoomTypes()
        {
            var controller = new RoomTypesController(_repository);
            var result = await controller.GetRoomTypes();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<RoomType>>(okResult.Value);
            Assert.Equal(4, returnValue.Count);
        }

    }

}