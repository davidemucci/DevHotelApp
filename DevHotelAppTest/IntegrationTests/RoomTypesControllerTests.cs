using AutoMapper;
using DevHotelAPI.Contexts;
using DevHotelAPI.Controllers;
using DevHotelAPI.Dtos;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using DevHotelAPI.Services.Repositories;
using DevHotelAPI.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DevHotelAppTest.IntegrationTests
{
    [Collection("DatabaseCollection")]
    public class RoomTypesControllerTests : IClassFixture<DatabaseFixture>
    {

        private readonly RoomTypesController _controller;
        private readonly DatabaseFixture _databaseFixture;

        public RoomTypesControllerTests(DatabaseFixture databaseFixture)
        {
            databaseFixture.ResetContext();
            _databaseFixture = databaseFixture;
            _controller = new RoomTypesController(DatabaseFixture.GetMapper(), new RoomTypeRepository(_databaseFixture.Context), new RoomTypeValidator(), _databaseFixture.Logger);
        }

        [Fact]
        public async Task DeleteRoom_ReturnNoContent()
        {
            //Arrange
            int roomId = _databaseFixture.roomTypesId.First();
            //Act
            var result = await _controller.DeleteRoomType(roomId);
            //Assert
            Assert.IsType<NoContentResult>(result);
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
            var roomId = _databaseFixture.roomTypesId.First();

            //Act
            var result = await _controller.GetRoomType(roomId);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<RoomType>(okResult.Value);

        }

        [Fact]
        public async Task GetRoomTypes_ReturnsAllRoomTypes()
        {
            // Act
            var result = await _controller.GetRoomTypes();
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<RoomType>>(okResult.Value);
            Assert.Equal(_databaseFixture.roomTypesId.Count, returnValue.Count);
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
        public async Task PutRoomType_ReturnsBadRequest_WhenDtoIsNotValid()
        {
            
            var roomType = new RoomTypeDto
            {
                Id = _databaseFixture.roomTypesId.First(),
                TotalNumber = 100
            };
            var result = await _controller.PutRoomType((int)roomType.Id, roomType);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutRoomType_ReturnsBadRequest_WhenIdDoesNotMatch()
        {
            // Arrange
            var roomType = new RoomTypeDto
            {
                Id = 1,
                TotalNumber = 200,
                Description = "Dscrizione"
            };

            // Act
            var result = await _controller.PutRoomType(3, roomType);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutRoomType_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            var roomType = new RoomTypeDto
            {
                Id = _databaseFixture.roomTypesId.First(),
                Description = "New King Room",
                TotalNumber = 100
            };
            var result = await _controller.PutRoomType((int)roomType.Id, roomType);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutRoomType_ReturnsNotFound_WhenRoomTypeDoesNotExist()
        {
            // Arrange
            var roomType = new RoomTypeDto { Id = 999, Description = "Descrizione", TotalNumber = 111 };

            // Act
            var result = await _controller.PutRoomType(999, roomType);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PutRoomType_ReturnsBadRequest_WhenRoomTypeIdIsZero()
        {
            // Arrange
            var roomType = new RoomTypeDto { Id = 0, Description = "Descrizione", TotalNumber = 111 };

            // Act
            var result = await _controller.PutRoomType(0, roomType);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}