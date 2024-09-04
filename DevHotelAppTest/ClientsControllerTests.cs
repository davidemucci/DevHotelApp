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
    public class ClientsControllerTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly HotelDevContext _context;
        private readonly ClientsController _controller;
        private readonly IMapper _mapper;
        private readonly IClientRepository _repository;
        private readonly IValidator<Client> _validator;

        public ClientsControllerTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            databaseFixture.ResetContext();
            databaseFixture.SeedDatabase();
            _context = databaseFixture._context;
            _mapper = databaseFixture.GetMapper();
            _repository = new ClientRepository(_context);
            _validator = new ClientValidator();
            _controller = new ClientsController(_mapper, _repository, _validator);
        }

        [Fact]
        public async Task GetClients_ReturnsAllClients()
        {
            // Act
            var result = await _controller.GetClients();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<ClientDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count); 
        }

        [Fact]
        public async Task GetClient_ReturnsClient_WhenClientExists()
        {
            // Arrange
            var clientId = Guid.Parse("22222222-2222-2222-2222-222222222221"); 

            // Act
            var result = await _controller.GetClient(clientId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ClientDto>(okResult.Value);
            Assert.Equal(clientId, returnValue.Id);
        }

        [Fact]
        public async Task GetClient_ReturnsNotFound_WhenClientDoesNotExist()
        {
            // Arrange
            var clientId = Guid.NewGuid();

            // Act
            var result = await _controller.GetClient(clientId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PutClient_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var clientId = Guid.Parse("22222222-2222-2222-2222-222222222221");
            var clientDto = new ClientDto { 
                Id = clientId, 
                Name = "Updated Client",
                Email = "newclientemail@email.com",
                Password = "password1234!",
                Address = "via cipressi 12, Milano, Italia"
            
            };

            // Act
            var result = await _controller.PutClient(clientId, clientDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PostClient_CreatesClient_WhenModelIsValid()
        {
            // Arrange
            var clientDto = new ClientDto
            {
                Name = "Updated Client",
                Email = "newclientemail@email.com",
                Password = "password1234!",
                Address = "via cipressi 12, Milano, Italia"
            };

            // Act
            var result = await _controller.PostClient(clientDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<ClientDto>(createdAtActionResult.Value);
            Assert.Equal(clientDto.Name, returnValue.Name);
            Assert.Equal(clientDto.Email, returnValue.Email);

        }

        [Fact]
        public async Task DeleteClient_ReturnsNoContent_WhenClientIsDeleted()
        {
            // Arrange
            var clientId = Guid.Parse("22222222-2222-2222-2222-222222222221");

            // Act
            var result = await _controller.DeleteClient(clientId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }

}
