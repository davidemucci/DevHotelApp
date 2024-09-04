using AutoMapper;
using DevHotelAPI.Contexts;
using DevHotelAPI.Controllers;
using DevHotelAPI.Dtos;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using DevHotelAPI.Services.Repositories;
using DevHotelAPI.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevHotelAppTest
{
    public class RoomsControllerTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly HotelDevContext _context;
        private readonly RoomsController _controller;
        private readonly IMapper _mapper;
        private readonly IRoomRepository _repository;
        private readonly IValidator<Room> _validator;

        public RoomsControllerTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            databaseFixture.ResetContext();
            databaseFixture.SeedDatabase();
            _context = databaseFixture._context;
            _mapper = databaseFixture.GetMapper();
            _repository = new RoomRepository(_context);
            _validator = new RoomValidator();
            _controller = new RoomsController(_mapper, _repository, _validator);
        }

        [Fact]
        public async Task GetRooms_ReturnsAllRooms()
        {
            // Act
            var result = await _controller.GetRooms();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<RoomDto>>(okResult.Value);
            Assert.Equal(10, returnValue.Count);
        }

        [Fact]
        public async Task GetRoom_ReturnsRoom_WhenRoomExists()
        {
            // Arrange
            var roomId = 100;

            // Act
            var result = await _controller.GetRoom(roomId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<RoomDto>(okResult.Value);
            Assert.Equal(roomId, returnValue.Number);
        }

        [Fact]
        public async Task GetRoom_ReturnsNotFound_WhenRoomDoesNotExist()
        {
            // Arrange
            var roomId = 999;

            // Act
            var result = await _controller.GetRoom(roomId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PutRoom_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var roomId = 100; 
            var roomDto = new RoomDto { Number = roomId, Description = "Updated Room", RoomTypeId = 2 };

            // Act
            var result = await _controller.PutRoom(roomId, roomDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PostRoom_CreatesRoom_WhenModelIsValid()
        {
            // Arrange
            var roomDto = new RoomDto { Description = "New Room", RoomTypeId = 1 };

            // Act
            var result = await _controller.PostRoom(roomDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<RoomDto>(createdAtActionResult.Value);
            Assert.Equal(roomDto.Description, returnValue.Description);
        }

        [Fact]
        public async Task DeleteRoom_ReturnsNoContent_WhenRoomIsDeleted()
        {
            // Arrange
            var roomId = 100;

            // Act
            var result = await _controller.DeleteRoom(roomId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }

}
