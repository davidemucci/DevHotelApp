using AutoMapper;
using DevHotelAPI.Contexts;
using DevHotelAPI.Services;
using DevHotelAPI.Services.Contracts;
using DevHotelAPI.Services.Mapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevHotelAppTest
{
    public class DatabaseFixture : IAsyncLifetime
    {
        public HotelDevContext _context { get; private set; }
        private IBogusRepository _bogusRepo;

        public DatabaseFixture()
        {
            
        }

        public async Task ResetContext()
        {
            var options = new DbContextOptionsBuilder<HotelDevContext>()
                .UseInMemoryDatabase(databaseName: "TestHotelDevDb" + Guid.NewGuid())
                .EnableSensitiveDataLogging()
                .Options;

            _bogusRepo = new BogusRepository();
            _context = new HotelDevContext(options, _bogusRepo);
            DetachAllEntities();
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        public async Task SeedDatabase()
        {
            var roomTypesFaker = _bogusRepo.GenerateRoomTypes();
            var roomsFaker = _bogusRepo.GenerateRooms(4, 10);
            var clientsFaker = _bogusRepo.GenerateClients();
            var reservationsFaker = _bogusRepo.GenerateReservations(clientsFaker);

            await _context.AddRangeAsync(roomTypesFaker);
            await _context.AddRangeAsync(roomsFaker);
            await _context.AddRangeAsync(clientsFaker);
            await _context.AddRangeAsync(reservationsFaker);
            await _context.SaveChangesAsync();
            DetachAllEntities();
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
             await ResetContext();
        }

        public async Task DisposeAsync()
        {
            _context.Database.EnsureDeleted();
            await _context.DisposeAsync();
        }
    }
}
