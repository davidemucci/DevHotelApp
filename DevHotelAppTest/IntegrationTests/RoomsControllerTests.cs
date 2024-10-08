﻿using AutoMapper;
using DevHotelAPI.Contexts;
using DevHotelAPI.Controllers;
using DevHotelAPI.Dtos;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using DevHotelAPI.Services.Repositories;
using DevHotelAPI.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevHotelAppTest.IntegrationTests
{
    public class RoomsControllerTests : IClassFixture<DatabaseFixture>
    {
        private readonly RoomsController _controller;
        private readonly DatabaseFixture _databaseFixture;

        public RoomsControllerTests(DatabaseFixture databaseFixture)
        {
            databaseFixture.ResetContext();
            _databaseFixture = databaseFixture;
            _controller = new RoomsController(DatabaseFixture.GetMapper(), new RoomRepository(_databaseFixture.Context), new RoomValidator(), _databaseFixture.Logger);
        }

        [Fact]
        public async Task DeleteRoom_ReturnsNoContent_WhenRoomIsDeleted()
        {
            // Arrange
            var roomId = _databaseFixture.roomsId.First();

            // Act
            var result = await _controller.DeleteRoom(roomId);

            // Assert
            Assert.IsType<NoContentResult>(result);
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
        public async Task GetRoom_ReturnsRoom_WhenRoomExists()
        {
            // Arrange
            var roomId = _databaseFixture.roomsId.First();

            // Act
            var result = await _controller.GetRoom(roomId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<RoomDto>(okResult.Value);
            Assert.Equal(roomId, returnValue.Number);
        }

        [Fact]
        public async Task GetRooms_ReturnsAllRooms()
        {
            // Act
            var result = await _controller.GetRooms();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<RoomDto>>(okResult.Value);
            Assert.Equal(_databaseFixture.roomsId.Count, returnValue.Count);
        }

        [Fact]
        public async Task GetRoomsAvailable_ReturnsAvailableRooms_WhenRoomsExistAndDateAreAvailable()
        {
            // Arrange
            var fromDate = new DateTime(2024, 9, 1);
            var toDate = new DateTime(2024, 9, 10);
            var people = 2;

            // Act
            var result = await _controller.GetRoomsAvailable(fromDate, toDate, people);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<IGrouping<int?, RoomDto>>>(okResult.Value);
            Assert.Equal(9, returnValue.Sum(x => x.Count()));
        }

        [Fact]
        public async Task GetRoomsAvailable_ReturnsAvailableRooms_WhenRoomsExistAndDateAreAvailable2()
        {
            // Arrange
            var fromDate = new DateTime(2024, 9, 1);
            var toDate = new DateTime(2024, 9, 10);
            var people = 4;

            // Act
            var result = await _controller.GetRoomsAvailable(fromDate, toDate, people);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<IGrouping<int?, RoomDto>>>(okResult.Value);
            Assert.Equal(5, returnValue.Sum(g => g.Count()));
        }


        [Fact]
        public async Task GetRoomsAvailable_ReturnsNoRooms_WhenRoomsExistAndDateArentAvailable()
        {
            // Arrange
            var fromDate = new DateTime(2027, 1, 16, 15, 0, 0);
            var toDate = new DateTime(2027, 1, 18, 15, 0, 0);
            var people = 2;

            // Act
            var result = await _controller.GetRoomsAvailable(fromDate, toDate, people);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<IGrouping<int?, RoomDto>>>(okResult.Value);
            Assert.Equal(5, returnValue.Sum(x => x.Count()));
        }

        [Fact]
        public async Task GetRoomsAvailable_ReturnsNoRooms_WhenRoomsExistAndDateArentAvailable2()
        {
            // Arrange
            var fromDate = new DateTime(2027, 1, 16, 15, 0, 0);
            var toDate = new DateTime(2027, 1, 18, 15, 0, 0);
            var people = 4;

            // Act
            var result = await _controller.GetRoomsAvailable(fromDate, toDate, people);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<IGrouping<int?, RoomDto>>>(okResult.Value);
            Assert.Equal(3, returnValue.Sum(g => g.Count()));
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
        public async Task PutRoom_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var roomId = _databaseFixture.roomsId.First(); ;
            var roomDto = new RoomDto { Number = roomId, Description = "Updated Room", RoomTypeId = 2 };

            // Act
            var result = await _controller.PutRoom(roomId, roomDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }

}
