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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevHotelAppTest
{
    public class ReservationsControllerTests : IClassFixture<DatabaseFixture>
    {
        private readonly HotelDevContext _context;
        private readonly ReservationsController _controller;
        private readonly DatabaseFixture _databaseFixture;
        private readonly IMapper _mapper;
        private readonly IReservationRepository _repository;
        private readonly IValidator<Reservation> _validator;

        public ReservationsControllerTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            databaseFixture.ResetContext();
            databaseFixture.SeedDatabase();
            _context = databaseFixture._context;
            _mapper = databaseFixture.GetMapper();
            _repository = new ReservationRepository(_context);
            _validator = new ReservationValidator();
            _controller = new ReservationsController(_mapper, _repository, _validator);
        }

        [Fact]
        public async Task DeleteReservation_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            // Arrange
            var validId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Act
            var result = await _controller.DeleteReservation(validId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteReservation_ReturnsNotFound_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            // Act
            var result = await _controller.DeleteReservation(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetReservation_ReturnsNotFound_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            // Act
            var result = await _controller.GetReservation(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetReservation_ReturnsReservation_WhenIdIsValid()
        {
            // Arrange
            var validId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Act
            var result = await _controller.GetReservation(validId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ReservationDto>(okResult.Value);
            Assert.Equal(validId, returnValue.Id);
        }

        [Fact]
        public async Task GetReservationByClientId_ReturnsOkResult_WithListOfReservationDtos()
        {
            // Arrange
            var clientId = Guid.Parse("22222222-2222-2222-2222-222222222221");

            // Act
            var result = await _controller.GetReservationByClientId(clientId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<ReservationDto>>(okResult.Value);
            Assert.Equal(3, returnValue.Count);
        }

        [Fact]
        public async Task GetReservations_ReturnsAllReservations()
        {
            // Act
            var result = await _controller.GetReservations();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<ReservationDto>>(okResult.Value);
            Assert.Equal(5, returnValue.Count); // Adjust the expected count as needed
        }
        [Fact]
        public async Task PostReservation_ReturnsBadRequest_WhenReservationDatesAreNotValid()
        {
            // Arrange
            var reservationDto = new ReservationDto
            {
                ClientId = Guid.Parse("22222222-2222-2222-2222-222222222221"),
                From = DateTime.Now.AddDays(1),
                To = DateTime.Now,
                RoomNumber = 102
            }; ;

            // Act
            var result = await _controller.PostReservation(reservationDto);

            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorResponse = Assert.IsType<List<ValidationFailure>>(actionResult.Value);
            Assert.Equal("To date must be later than From date.", errorResponse.Select(e => e.ErrorMessage).Where(m => m == "To date must be later than From date.").FirstOrDefault());
        }

        [Fact]
        public async Task PostReservation_ReturnsBadRequest_WhenReservationIsExactMatch()
        {
            // Arrange
            var reservationDto = new ReservationDto
            {
                ClientId = Guid.Parse("22222222-2222-2222-2222-222222222225"),
                From = new DateTime(2027, 1, 16, 15, 15, 0),
                To = new DateTime(2027, 1, 18, 15, 15, 0),
                RoomNumber = 100
            };

            // Act
            var result = await _controller.PostReservation(reservationDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("The selected room is not available for the specified dates. Please choose different dates or another room.", badRequestResult.Value);
        }

        [Fact]
        public async Task PostReservation_ReturnsBadRequest_WhenReservationIsFullyContained()
        {
            // Arrange
            var reservationDto = new ReservationDto
            {
                ClientId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                From = new DateTime(2027, 1, 17, 15, 0, 0),
                To = new DateTime(2027, 1, 18, 15, 0, 0),
                RoomNumber = 100
            };

            // Act
            var result = await _controller.PostReservation(reservationDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("The selected room is not available for the specified dates. Please choose different dates or another room.", badRequestResult.Value);
        }

        [Fact]
        public async Task PostReservation_ReturnsBadRequest_WhenReservationIsNotValid()
        {
            // Arrange
            var reservationDto = new ReservationDto
            {
                ClientId = Guid.Parse("22222222-2222-2222-2222-222222222221"),
                From = DateTime.Now.AddDays(1),
                RoomNumber = 102
            }; ;

            // Act
            var result = await _controller.PostReservation(reservationDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostReservation_ReturnsBadRequest_WhenReservationOverlapsBothEnds()
        {
            // Arrange
            var reservationDto = new ReservationDto
            {
                ClientId = Guid.Parse("22222222-2222-2222-2222-222222222223"),
                From = new DateTime(2027, 1, 16, 15, 0, 0),
                To = new DateTime(2027, 1, 18, 15, 0, 0),
                RoomNumber = 100
            };

            // Act
            var result = await _controller.PostReservation(reservationDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("The selected room is not available for the specified dates. Please choose different dates or another room.", badRequestResult.Value);
        }

        [Fact]
        public async Task PostReservation_ReturnsBadRequest_WhenReservationOverlapsEnd()
        {
            // Arrange
            var reservationDto = new ReservationDto
            {
                ClientId = Guid.Parse("22222222-2222-2222-2222-222222222224"),
                From = new DateTime(2027, 1, 17, 15, 0, 0),
                To = new DateTime(2027, 1, 19, 15, 0, 0),
                RoomNumber = 100
            };

            // Act
            var result = await _controller.PostReservation(reservationDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("The selected room is not available for the specified dates. Please choose different dates or another room.", badRequestResult.Value);
        }

        [Fact]
        public async Task PostReservation_ReturnsBadRequest_WhenReservationOverlapsStart()
        {
            // Arrange
            var reservationDto = new ReservationDto
            {
                ClientId = Guid.Parse("22222222-2222-2222-2222-222222222221"),
                From = new DateTime(2027, 1, 15, 15, 0, 0),
                To = new DateTime(2027, 1, 17, 15, 0, 0),
                RoomNumber = 100
            };

            // Act
            var result = await _controller.PostReservation(reservationDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("The selected room is not available for the specified dates. Please choose different dates or another room.", badRequestResult.Value);
        }

        [Fact]
        public async Task PostReservation_ReturnsCreatedAtAction_WhenReservationIsValid()
        {
            // Arrange
            var reservationDto = new ReservationDto
            {
                ClientId = Guid.Parse("22222222-2222-2222-2222-222222222221"),
                From = DateTime.Now.AddDays(1),
                To = DateTime.Now.AddDays(3),
                RoomNumber = 102
            }; ;

            // Act
            var result = await _controller.PostReservation(reservationDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<ReservationDto>(createdAtActionResult.Value);
            Assert.Equal(reservationDto.Id, returnValue.Id);
        }

        [Fact]
        public async Task PutReservation_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var validId = Guid.NewGuid();
            var reservationDto = new ReservationDto
            {
                Id = Guid.NewGuid(),
                ClientId = Guid.Parse("22222222-2222-2222-2222-222222222221"),
                From = DateTime.Now.AddDays(1),
                To = DateTime.Now.AddDays(3),
                RoomNumber = 101
            };

            // Act
            var result = await _controller.PutReservation(validId, reservationDto);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutReservation_ReturnsBadRequest_WhenReservationOverlapsBothEnds()
        {
            // Arrange
            var validId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var reservationDto = new ReservationDto
            {
                Id = validId,
                ClientId = Guid.Parse("22222222-2222-2222-2222-222222222223"),
                From = new DateTime(2027, 1, 16, 15, 0, 0),
                To = new DateTime(2027, 1, 18, 15, 0, 0),
                RoomNumber = 100
            };

            // Act
            var result = await _controller.PutReservation(validId, reservationDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The selected room is not available for the specified dates. Please choose different dates or another room.", badRequestResult.Value);
        }

        [Fact]
        public async Task PutReservation_ReturnsBadRequest_WhenReservationOverlapsEnd()
        {
            // Arrange
            var validId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var reservationDto = new ReservationDto
            {
                Id = validId,
                ClientId = Guid.Parse("22222222-2222-2222-2222-222222222224"),
                From = new DateTime(2027, 1, 17, 15, 0, 0),
                To = new DateTime(2027, 1, 19, 15, 0, 0),
                RoomNumber = 100
            };

            // Act
            var result = await _controller.PutReservation(validId, reservationDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The selected room is not available for the specified dates. Please choose different dates or another room.", badRequestResult.Value);
        }

        [Fact]
        public async Task PutReservation_ReturnsBadRequest_WhenReservationOverlapsStart()
        {
            var validId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            // Arrange
            var reservationDto = new ReservationDto
            {
                Id = validId,
                ClientId = Guid.Parse("22222222-2222-2222-2222-222222222221"),
                From = new DateTime(2027, 1, 15, 15, 0, 0),
                To = new DateTime(2027, 1, 17, 15, 0, 0),
                RoomNumber = 100
            };

            // Act
            var result = await _controller.PutReservation(validId, reservationDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The selected room is not available for the specified dates. Please choose different dates or another room.", badRequestResult.Value);
        }

        [Fact]
        public async Task PutReservation_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var validId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var reservationDto = new ReservationDto
            {
                Id = validId,
                ClientId = Guid.Parse("22222222-2222-2222-2222-222222222221"),
                From = DateTime.Now.AddDays(1),
                To = DateTime.Now.AddDays(3),
                RoomNumber = 101
            };

            // Act
            var result = await _controller.PutReservation(validId, reservationDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}