using AutoMapper;
using DevHotelAPI.Contexts;
using DevHotelAPI.Contexts.Identity;
using DevHotelAPI.Services;
using DevHotelAPI.Services.Contracts;
using DevHotelAPI.Services.Mapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;

namespace DevHotelAppTest
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private IBogusRepository _bogusRepo;
        private UserManager<IdentityUser<Guid>> _userManger;
        public HotelDevContext _context { get; private set; }
        public IdentityContext _identityContext { get; private set; }
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
            _identityContext.Database.EnsureCreated();
            _context.Database.EnsureCreated();
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
