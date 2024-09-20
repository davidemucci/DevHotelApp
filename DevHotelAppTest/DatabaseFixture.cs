using AutoMapper;
using DevHotelAPI.Contexts;
using DevHotelAPI.Contexts.Identity;
using DevHotelAPI.Services.Contracts;
using DevHotelAPI.Services.Mapper;
using DevHotelAPI.Services.Repositories;
using DevHotelAPI.Services.Utility;
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
        private readonly BogusRepository BogusRepo;
        public HotelDevContext Context { get; private set; }
        public IdentityContext IdentityContext { get; private set; }
        public UserManager<IdentityUser<Guid>> _userManager;
        public ILogger Logger { get; private set; }
        public IHandleExceptionService HandleExceptionService { get; private set; }


        public string userNameAdmin;
        public string userNameConsumer;
        public Guid consumerId;
        public Guid consumerIdentityId;
        public Guid adminId;
        public Guid adminIdentityId;
        public List<Guid> reservationsId;
        public List<int> roomsId;
        public List<int> roomTypesId;

        public DatabaseFixture()
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

            BogusRepo = new BogusRepository();
            Context = new HotelDevContext(options, BogusRepo, env);
            IdentityContext = new IdentityContext(optionsIdentity, BogusRepo, env);

        }

        public void DetachAllEntities()
        {
            var changedEntriesCopy = Context.ChangeTracker.Entries()
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
            IdentityContext.Database.EnsureDeleted();
            Context.Database.EnsureDeleted();

            await Context.DisposeAsync();
            await IdentityContext.DisposeAsync();
        }

        public static IMapper GetMapper()
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
            IdentityContext.Database.EnsureDeleted();
            Context.Database.EnsureDeleted();

            IdentityContext.Database.EnsureCreated();
            Context.Database.EnsureCreated();

            var identityStore = new UserStore<IdentityUser<Guid>, IdentityRole<Guid>, IdentityContext, Guid>(IdentityContext);
            _userManager = new UserManager<IdentityUser<Guid>>(identityStore, null!, new PasswordHasher<IdentityUser<Guid>>(),
                new List<IUserValidator<IdentityUser<Guid>>> { new UserValidator<IdentityUser<Guid>>() }
                , null!, new UpperInvariantLookupNormalizer(), null!, null!, null!);
            Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();
            HandleExceptionService = new HandleExceptionService(Logger);


            var roles = IdentityContext.Roles.ToList();
            consumerIdentityId = BogusRepo.ConsumerIdentityId;
            adminIdentityId = BogusRepo.AdminIdenityId;
            userNameAdmin = BogusRepo.UserNameAdmin;
            userNameConsumer = BogusRepo.UserNameConsumer;
            consumerId = BogusRepo.ConsumerId;
            adminId = BogusRepo.AdminId;
            reservationsId = BogusRepo.ReservationsId;
            roomsId = BogusRepo.RoomsId;
            roomTypesId = BogusRepo.RoomTypesId;

            DetachAllEntities();
        }

        public void SetHttpContextAsAdminUser(ControllerBase controller)
        {
            var userAdmin = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new (ClaimTypes.Name, userNameAdmin)
            ], "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userAdmin }
            };
        }

        public void SetHttpContextAsConsumerUser(ControllerBase controller)
        {
            var userConsumer = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new (ClaimTypes.Name, userNameConsumer)
            ], "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userConsumer }
            };
        }

        public class HostEnvironment : IHostEnvironment
        {
            public string EnvironmentName { get; set; } = string.Empty;
            public string ApplicationName { get; set; } = string.Empty;
            public string ContentRootPath { get; set; } = string.Empty;
            IFileProvider IHostEnvironment.ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        }
    }
}
