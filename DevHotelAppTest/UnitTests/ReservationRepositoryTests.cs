using Bogus;
using DevHotelAPI.Contexts.Identity;
using DevHotelAPI.Contexts;
using DevHotelAPI.Entities;
using DevHotelAPI.Services.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevHotelAPI.Controllers;
using DevHotelAPI.Services;
using DevHotelAPI.Validators;
using Microsoft.AspNetCore.Mvc;

namespace DevHotelAppTest.UnitTests
{
    public class ReservationRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly HotelDevContext _context;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly ReservationRepository _repository;

        public ReservationRepositoryTests(DatabaseFixture _databaseFixture)
        {
            _databaseFixture.ResetContext();
            _context = _databaseFixture.Context;
            _userManager = _databaseFixture._userManager;
            _repository = new ReservationRepository(_context, _databaseFixture.IdentityContext, _userManager);
        }

        [Fact]
        public async Task AddReservationAsync_ShouldAddReservation()
        {
            // Arrange
            var user = await _userManager.FindByNameAsync("CONSUMER");


            var customer = new Faker<Customer>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.IdentityUserId, f => user?.Id)
                .Generate();

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            var reservation = new Faker<Reservation>()
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.CustomerId, f => customer.Id)
                .RuleFor(r => r.From, f => f.Date.Future())
                .RuleFor(r => r.To, f => f.Date.Future())
                .RuleFor(r => r.RoomNumber, f => f.Random.Int(100, 999))
                .Generate();

            // Act
            await _repository.AddReservationAsync(reservation, "CONSUMER");

            // Assert
            var addedReservation = await _context.Reservations.FindAsync(reservation.Id);
            Assert.NotNull(addedReservation);
            Assert.Equal(reservation.Id, addedReservation.Id);
        }

        [Fact]
        public async Task CheckIfRoomIsAvailableAsync_ShouldReturnTrueIfRoomIsAvailable()
        {
            // Arrange
            var reservation = new Faker<Reservation>()
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.From, f => f.Date.Future())
                .RuleFor(r => r.To, f => f.Date.Future())
                .RuleFor(r => r.RoomNumber, f => f.Random.Int(100, 999))
                .Generate();

            // Act
            var isAvailable = await _repository.CheckIfRoomIsAvailableAsync(reservation);

            // Assert
            Assert.True(isAvailable);
        }

        [Fact]
        public async Task DeleteReservationAsync_ShouldRemoveReservation()
        {
            // Arrange
            var user = await _userManager.FindByNameAsync("CONSUMER");

            var customer = new Faker<Customer>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.IdentityUserId, f => user?.Id)
                .Generate();

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            var reservation = new Faker<Reservation>()
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.CustomerId, f => customer.Id)
                .RuleFor(r => r.From, f => f.Date.Future())
                .RuleFor(r => r.To, f => f.Date.Future())
                .RuleFor(r => r.RoomNumber, f => f.Random.Int(100, 999))
                .Generate();

            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteReservationAsync(reservation.Id, "CONSUMER");

            // Assert
            var deletedReservation = await _context.Reservations.FindAsync(reservation.Id);
            Assert.Null(deletedReservation);
        }

        [Fact]
        public async Task GetAllReservationsAsync_ShouldReturnAllReservations()
        {
            // Arrange
            var reservations = new Faker<Reservation>()
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.From, f => f.Date.Future())
                .RuleFor(r => r.To, f => f.Date.Future())
                .RuleFor(r => r.RoomNumber, f => f.Random.Int(100, 999))
                .Generate(5);
            var count = _context.Reservations.Count();

            await _context.Reservations.AddRangeAsync(reservations);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllReservationsAsync();

            // Assert
            Assert.Equal(5 + count, result.Count());
        }

        [Fact]
        public async Task UpdateReservationAsync_ShouldUpdateReservation()
        {
            // Arrange
            var user = await _userManager.FindByNameAsync("CONSUMER");

            var customer = new Faker<Customer>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.IdentityUserId, f => user?.Id)
                .Generate();

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            var reservation = new Faker<Reservation>()
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.CustomerId, f => customer.Id)
                .RuleFor(r => r.From, f => f.Date.Future())
                .RuleFor(r => r.To, f => f.Date.Future())
                .RuleFor(r => r.RoomNumber, f => f.Random.Int(100, 999))
                .Generate();

            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            reservation.To = reservation.To.AddDays(1); // Update the reservation

            // Act
            await _repository.UpdateReservationAsync(reservation, "CONSUMER");

            // Assert
            var updatedReservation = await _context.Reservations.FindAsync(reservation.Id);
            Assert.NotNull(updatedReservation);
            Assert.Equal(reservation.To, updatedReservation.To);
        }
    }
}
