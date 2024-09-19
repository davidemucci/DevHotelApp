using AutoMapper;
using DevHotelAPI.Contexts;
using DevHotelAPI.Contexts.Identity;
using DevHotelAPI.Controllers;
using DevHotelAPI.Dtos;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Contracts;
using DevHotelAPI.Services.Repositories;
using DevHotelAPI.Services.Utility;
using DevHotelAPI.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Core;
using System.Security.Claims;

namespace DevHotelAppTest.IntegrationTests
{
    public class ReservationsControllerTests : IClassFixture<DatabaseFixture>
    {
        private readonly ReservationsController _controller;
        private readonly DatabaseFixture _databaseFixture;

        public ReservationsControllerTests(DatabaseFixture databaseFixture)
        {
            databaseFixture.ResetContext();
            _databaseFixture = databaseFixture;
            _controller = new ReservationsController(_databaseFixture._handleExceptionService, _databaseFixture.GetMapper(),
                new ReservationRepository(_databaseFixture._context, _databaseFixture._identityContext, _databaseFixture._userManager),
                new ReservationValidator(), _databaseFixture._logger);
            _databaseFixture.SetHttpContextAsConsumerUser(_controller);
        }

        [Fact]
        public async Task DeleteReservation_ReturnsNotFound_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            // Act
            var result = await _controller.DeleteReservation(invalidId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteReservationOfOtherCustomerAsAdmin_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            // Arrange
            var validId = _databaseFixture.reservationsId.Last();
            _databaseFixture.SetHttpContextAsAdminUser(_controller);

            // Act
            var result = await _controller.DeleteReservation(validId);

            // Assert
            Assert.IsType<NoContentResult>(result);
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
            var validId = _databaseFixture._context.Reservations.Where(c => c.CustomerId.Equals(_databaseFixture.consumerId)).Select(c => c.Id).First();

            // Act
            var result = await _controller.GetReservation(validId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ReservationDto>(okResult.Value);
            Assert.Equal(validId, returnValue.Id);
        }

        [Fact]
        public async Task GetReservationByCustomerId_ReturnsOkResult_WithListOfReservationDtos()
        {
            // Arrange
            var customerId = _databaseFixture.consumerId;

            var userConsumer = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "CONSUMER")
                }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userConsumer }
            };

            // Act
            var result = await _controller.GetReservationByCustomerId(customerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<ReservationDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetReservations_ReturnsAllReservations()
        {
            // Act
            var result = await _controller.GetReservations();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<ReservationDto>>(okResult.Value);
            Assert.Equal(_databaseFixture.reservationsId.Count, returnValue.Count); // Adjust the expected count as needed
        }
        [Fact]
        public async Task PostReservation_ReturnsBadRequest_WhenReservationDatesAreNotValid()
        {
            // Arrange
            var reservationDto = new ReservationDto
            {
                CustomerId = Guid.Parse("22222222-2222-2222-2222-222222222221"),
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
                CustomerId = _databaseFixture.consumerId,
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
                CustomerId = _databaseFixture.consumerId,
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
                CustomerId = _databaseFixture.adminId,
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
                CustomerId = _databaseFixture.consumerId,
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
                CustomerId = _databaseFixture.consumerId,
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
                CustomerId = Guid.Parse("22222222-2222-2222-2222-222222222221"),
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
                CustomerId = _databaseFixture.consumerId,
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
                CustomerId = _databaseFixture.consumerId,
                From = DateTime.Now.AddDays(1),
                To = DateTime.Now.AddDays(3),
                RoomNumber = 101
            };

            // Act
            var result = await _controller.PutReservation(validId, reservationDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutReservation_ReturnsBadRequest_WhenReservationOverlapsBothEnds()
        {
            // Arrange
            var validId = _databaseFixture.reservationsId.Last();
            var reservationDto = new ReservationDto
            {
                Id = validId,
                CustomerId = _databaseFixture.consumerId,
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
            var validId = _databaseFixture.reservationsId.Last();
            var reservationDto = new ReservationDto
            {
                Id = validId,
                CustomerId = _databaseFixture.consumerId,
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
            var validId = _databaseFixture.reservationsId.Last();
            // Arrange
            var reservationDto = new ReservationDto
            {
                Id = validId,
                CustomerId = _databaseFixture.consumerId,
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
            var validId = _databaseFixture.reservationsId.Last();
            var reservationDto = new ReservationDto
            {
                Id = validId,
                CustomerId = _databaseFixture.consumerId,
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