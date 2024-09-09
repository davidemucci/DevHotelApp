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

namespace DevHotelAppTest
{
    public class CustomersControllerTests : IClassFixture<DatabaseFixture>
    {
        private readonly HotelDevContext _context;
        private readonly CustomerController _controller;
        private readonly DatabaseFixture _databaseFixture;
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _repository;
        private readonly IValidator<Customer> _validator;

        public CustomersControllerTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            databaseFixture.ResetContext();
            databaseFixture.SeedDatabase();
            _context = databaseFixture._context;
            _mapper = databaseFixture.GetMapper();
            _repository = new CustomerRepository(_context);
            _validator = new CustomerValidator();
            _controller = new CustomerController(_mapper, _repository, _validator);
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsNoContent_WhenCustomerIsDeleted()
        {
            // Arrange
            var customerId = Guid.Parse("22222222-2222-2222-2222-222222222221");

            // Act
            var result = await _controller.DeleteCustomer(customerId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetCustomer_ReturnsCustomer_WhenCustomerExists()
        {
            // Arrange
            var customerId = Guid.Parse("22222222-2222-2222-2222-222222222221");

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
            Assert.IsType<NotFoundResult>(result.Result);
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
            var customerId = Guid.Parse("22222222-2222-2222-2222-222222222221");
            var customerDto = new CustomerDto { 
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
