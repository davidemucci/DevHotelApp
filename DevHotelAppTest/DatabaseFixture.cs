using AutoMapper;
using DevHotelAPI.Contexts;
using DevHotelAPI.Contexts.Identity;
using DevHotelAPI.Services;
using DevHotelAPI.Services.Contracts;
using DevHotelAPI.Services.Mapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Serilog;
using Serilog.Core;
using System.Security.Claims;

namespace DevHotelAppTest
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private IBogusRepository _bogusRepo;
        private UserManager<IdentityUser<Guid>> _userManger;
        public HotelDevContext _context { get; private set; }
        public IdentityContext _identityContext { get; private set; }
        public UserManager<IdentityUser<Guid>> _userManager;
        public ILogger _logger { get; private set; }
        public DatabaseFixture()
        {

        }

        public void DetachAllEntities()
        {
            var changedEntriesCopy = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted ||
                            e.State == EntityState.Unchanged)
                .ToList();

            foreach (var entry in changedEntriesCopy)
            {
                entry.State = EntityState.Detached;
            }
        }

        public async Task DisposeAsync()
        {
            _context.Database.EnsureDeleted();
            await _context.DisposeAsync();
        }

        public IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MainMapperProfile>();
            });
            return config.CreateMapper();
        }

        public async Task InitializeAsync()
        {
            ResetContext();
        }

        public void ResetContext()
        {
            var options = new DbContextOptionsBuilder<HotelDevContext>()
                .UseInMemoryDatabase(databaseName: "TestHotelDevDb-" + Guid.NewGuid())
                .EnableSensitiveDataLogging()
                .Options;

            var optionsIdentity = new DbContextOptionsBuilder<IdentityContext>()
                .UseInMemoryDatabase(databaseName: "TestIdentityHotelDevDb-" + Guid.NewGuid())
                .EnableSensitiveDataLogging()
                .Options;

            IHostEnvironment env = new HostEnvironment()
            {
                EnvironmentName = "Staging",
                ApplicationName = "App",
                ContentRootPath = ""
            };

            _bogusRepo = new BogusRepository();
            _context = new HotelDevContext(options, _bogusRepo, env);
            _identityContext = new IdentityContext(optionsIdentity, _bogusRepo, env);
            var identityStore = new UserStore<IdentityUser<Guid>, IdentityRole<Guid>, IdentityContext, Guid>(_identityContext);
            _userManager = new UserManager<IdentityUser<Guid>>(identityStore, null, null, null, null, null, null, null, null);
            _logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();

            _identityContext.Database.EnsureCreated();
            _context.Database.EnsureCreated();

        }

        public void SetHttpContextAsAdminUser(ControllerBase controller)
        {
            var userAdmin = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "ADMIN")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userAdmin }
            };
        }

        public void SetHttpContextAsConsumerUser(ControllerBase controller)
        {
            var userConsumer = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "CONSUMER")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userConsumer }
            };
        }

        public class HostEnvironment : IHostEnvironment
        {
            public string EnvironmentName { get; set; }
            public string ApplicationName { get; set; }
            public string ContentRootPath { get; set; }
            IFileProvider IHostEnvironment.ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        }
    }
}
