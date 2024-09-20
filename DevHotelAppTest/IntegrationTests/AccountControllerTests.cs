using Bogus.DataSets;
using DevHotelAPI.Controllers;
using DevHotelAPI.Entities;
using DevHotelAPI.Entities.Identity;
using DevHotelAPI.Services.Contracts;
using DevHotelAPI.Services.Repositories;
using DevHotelAPI.Services.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Moq;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevHotelAppTest.IntegrationTests
{
    public class AccountControllerTests : IClassFixture<DatabaseFixture>
    {
        private readonly AccountController _controller;
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly DatabaseFixture _databaseFixture;
        public AccountControllerTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            _databaseFixture.ResetContext();
            _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

            _accountRepository = new AccountRepository(_configuration, databaseFixture.Context ,databaseFixture._userManager);
            _controller = new AccountController(databaseFixture._userManager, _accountRepository, databaseFixture.IdentityContext);
        }

        [Fact]
        public async Task Login_InvalidEmailOrPassword_ReturnsBadRequest()
        {
            // Arrange
            var model = new LoginModel { Email = "test@example.com", Password = "wrongpassword" };

            // Act
            var result = await _controller.Login(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid email or password.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var model = new LoginModel { Email = "CONSUMER@EMAIL.COM", Password = "Passw0rd!" };
            // Act
            var result = await _controller.Login(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Type type = okResult.Value.GetType();
            PropertyInfo property = type.GetProperty("token");
            Assert.Equal("token", property.Name);
            Assert.Equal(okResult.StatusCode, 200);
        }

        [Fact]
        public async Task Register_UserNameAlreadyTaken_ReturnsBadRequest()
        {
            // Arrange
            var model = new AddOrUpdateCustomerModel { UserName = "ADMIN", Email = "ADMIN@EMAIL.COM", Password = "Password123!" };
            var existingUser = new IdentityUser<Guid> { UserName = "ADMIN" };

            // Act
            var result = await _controller.Register(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User name or email is already taken", badRequestResult.Value);
        }

        [Fact]
        public async Task Register_ValidUser_ReturnsOkWithToken()
        {
            // Arrange
            var model = new AddOrUpdateCustomerModel { UserName = "newUser", Email = "test@example.com", Password = "Password123!" };

            // Act
            var result = await _controller.Register(model);
            var user = _databaseFixture.IdentityContext.Users.Where(u => u.Email.Equals(model.Email)).First();
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True(_databaseFixture.Context.Customers.Any(c => c.Email.Equals(model.Email)));
            Assert.True(await _databaseFixture._userManager.IsInRoleAsync(user, HotelDevUtilities.Roles.Consumer.ToString()));   
            Assert.Equal(okResult.StatusCode, 200);
        }

        [Fact]
        public async Task AddRole_UserNotFound_ReturnsBadRequest()
        {
            // Arrange
            string username = "nonexistentuser";
            string role = "Administrator";

            // Act
            var result = await _controller.AddRole(username, role);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"User with {username} not found.", badRequestResult.Value);
        }

        [Fact]
        public async Task AddRole_ValidUser_ReturnsOk()
        {
            // Arrange
            string username = "test";
            string email = "test@email.com";
            string role = "Administrator";
            var user = new IdentityUser<Guid> { UserName = username, Email = email};
            await _databaseFixture._userManager.CreateAsync(user, "Passw0rd!");

            // Act
            var result = await _controller.AddRole(username, role);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddRoleNotValid_ReturnsBadRequest()
        {
            // Arrange
            string username = "test";
            string email = "test@email.com";
            string role = "Administratorss";
            var user = new IdentityUser<Guid> { UserName = username, Email = email };
            await _databaseFixture._userManager.CreateAsync(user, "Passw0rd!");

            // Act
            var result = await _controller.AddRole(username, role);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Role ADMINISTRATORSS does not exist.", badRequestResult.Value);
        }

    }
}
