using AutoMapper;
using DevHotelAPI.Contexts;
using DevHotelAPI.Dtos;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Mapper;
using DevHotelAPI.Services.Repositories;
using DevHotelAPI.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DevHotelAppTest
{
    [Collection("DatabaseCollection")]
    public class RoomTypesControllerTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly HotelDevContext _context;
        private readonly RoomTypesController _controller;
        private readonly IMapper _mapper;   
        private readonly IRoomTypeRepository _repository;
        private readonly IValidator<RoomType> _validator;

        public RoomTypesControllerTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            databaseFixture.ResetContext();
            databaseFixture.SeedDatabase();
            _context = databaseFixture._context;
            _mapper = databaseFixture.GetMapper();
            _repository = new RoomTypeRepository(_context);
            _validator = (IValidator<RoomType>)new RoomTypeValidator();
            _controller = new RoomTypesController(_context, _mapper, _repository, _validator);
        }





        [Fact]
        public async Task GetRoomTypes_ReturnsAllRoomTypes()
        {
            // Act
            var result = await _controller.GetRoomTypes();
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<RoomType>>(okResult.Value);
            Assert.Equal(4, returnValue.Count);
        }

        [Fact]
        public async Task GetRoomType_ReturnsNotFound_WhenRoomTypeDoesNotExist()
        {
            // Act
            var result = await _controller.GetRoomType(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Fact]
        public async Task GetRoomTypeById_ReturnsOk()
        {
            //Act
            var result = await _controller.GetRoomType(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<RoomType>(okResult.Value);

        }

        [Fact]
        public async Task PutRoomType_ReturnsBadRequest_WhenIdDoesNotMatch()
        {
            // Arrange
            var roomType = new RoomType { Id = 1 };
            roomType.TotalNumber = 200;

            // Act
            var result = await _controller.PutRoomType(3, roomType);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutRoomType_ReturnsNotFound_WhenRoomTypeDoesNotExist()
        {
            // Arrange
            var roomType = new RoomType { Id = 999 }; // Assuming 999 does not exist in the generated data

            // Act
            var result = await _controller.PutRoomType(999, roomType);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PutRoomType_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            var roomType = new RoomType
            {
                Id = 4,
                Description = "New King Room",
                TotalNumber = 100
            };
            var result = await _controller.PutRoomType(roomType.Id, roomType);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PostRoomType_ReturnsCreatedAtAction()
        {

            // Arrange
            var roomType = new RoomTypeDto
            {
                Description = "New King Room",
                TotalNumber = 100
            };
            // Act
            var result = await _controller.PostRoomType(roomType);
            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<RoomType>(actionResult.Value);
            Assert.Equal(roomType.Description, returnValue.Description);
            Assert.Equal(roomType.TotalNumber, returnValue.TotalNumber);
        }

        [Fact]
        public async Task PostRoomTypeNotValid_ReturnBadRequest()
        {
            // Arrange
            var roomTypeDto = new RoomTypeDto
            {
                Description = "Suite King XXL"
            };
            // Act
            var result = await _controller.PostRoomType(roomTypeDto);
            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorResponse = Assert.IsType<List<ValidationFailure>>(actionResult.Value);
            Assert.Equal("TotalNumber must be greater than one.", errorResponse.Where(x => x.PropertyName.Equals("TotalNumber")).Select(x => x.ErrorMessage).FirstOrDefault());

        }

        [Fact]
        public async Task DeleteRoom_ReturnNoContent()
        {
            //Arrange
            int roomId = 1;
            //Act
            var result = await _controller.DeleteRoomType(roomId);
            //Assert
            Assert.IsType<NoContentResult>(result);
        }

    }
}