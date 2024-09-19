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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DevHotelAppTest.IntegrationTests
{
    public class CustomersControllerTests : IClassFixture<DatabaseFixture>
    {
        private readonly CustomerController _controller;
        private readonly DatabaseFixture _databaseFixture;

        public CustomersControllerTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            _databaseFixture.ResetContext();
            _controller = new CustomerController(_databaseFixture._handleExceptionService, databaseFixture._logger, _databaseFixture.GetMapper(),
                new CustomerRepository(databaseFixture._context, databaseFixture._userManager),
                new CustomerValidator());
            _databaseFixture.SetHttpContextAsConsumerUser(_controller);
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsNoContent_WhenCustomerIsDeleted()
        {
            // Arrange
            var customerId = _databaseFixture.consumerId;
            //var customerId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            // Act
            var result = await _controller.DeleteCustomer(customerId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsBadRequest_WhenCustomerToDeleteIsAnotherUserANDIsNotAdmin()
        {
            // Arrange
            var adminId = _databaseFixture.adminId;

            // Act
            var result = await _controller.DeleteCustomer(adminId);
            var isNotDeleted = await _databaseFixture._context.Customers.AnyAsync(c => c.Id.Equals(adminId));

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.True(isNotDeleted);
        }

        [Fact]
        public async Task GetCustomer_ReturnsCustomer_WhenCustomerExists()
        {
            // Arrange
            var customerId = _databaseFixture.consumerId;

            // Act
            var result = await _controller.GetCustomer(customerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<CustomerDto>(okResult.Value);
            Assert.Equal(customerId, returnValue.Id);
        }

        [Fact]
        public async Task GetCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            // Act
            var result = await _controller.GetCustomer(customerId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetCustomers_ReturnsAllCustomers()
        {
            // Act
            var result = await _controller.GetCustomers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<CustomerDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
        [Fact]
        public async Task PostCustomer_CreatesCustomer_WhenModelIsValid()
        {
            // Arrange
            var customerDto = new CustomerDto
            {
                Name = "Updated Customer",
                Email = "newcustomeremail@email.com",
                Password = "password1234!",
                Address = "via cipressi 12, Milano, Italia"
            };

            // Act
            var result = await _controller.PostCustomer(customerDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<CustomerDto>(createdAtActionResult.Value);
            Assert.Equal(customerDto.Name, returnValue.Name);
            Assert.Equal(customerDto.Email, returnValue.Email);

        }

        [Fact]
        public async Task PutCustomer_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var customerId = _databaseFixture.consumerId;
            var customerDto = new CustomerDto
            {
                Id = customerId,
                Name = "Updated Customer",
                Email = "newcustomeremail@email.com",
                Password = "password1234!",
                Address = "via cipressi 12, Milano, Italia"
            };

            // Act
            var result = await _controller.PutCustomer(customerId, customerDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }

}
