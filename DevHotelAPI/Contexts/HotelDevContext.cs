using Bogus;
using DevHotelAPI.Entities;
using DevHotelAPI.Services;
using DevHotelAPI.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DevHotelAPI.Contexts
{
    public class HotelDevContext : DbContext
    {
        private IBogusRepository _bogusRepo {  get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }

        public HotelDevContext(DbContextOptions options, IBogusRepository bogusRepo) : base(options)
        {
            _bogusRepo = bogusRepo;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(p => p.Email).IsRequired();
                entity.Property(p => p.Password).IsRequired();
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasOne(e => e.Client).WithMany(e => e.Reservations)
                .IsRequired();
                entity.Property(e => e.From).IsRequired();
                entity.Property(e => e.To).IsRequired();
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Number);
                entity.HasOne(e => e.Type).WithMany()
                .HasForeignKey(r => r.RoomTypeId)
                .IsRequired();

            });

            modelBuilder.Entity<RoomType>(entity =>
            {
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.TotalNumber).IsRequired();
                entity.HasKey(e => e.Id);
            });

            if (!IsInMemoryDatabase())
                DbInitialize(modelBuilder);

            base.OnModelCreating(modelBuilder);

        }

        public void DbInitialize(ModelBuilder modelBuilder)
        {
            var roomTypesFaker = _bogusRepo.GenerateRoomTypes();
            var roomsFaker = _bogusRepo.GenerateRooms(4,10);
            var clientsFaker = _bogusRepo.GenerateClients();
            var reservationsFaker = _bogusRepo.GenerateReservations(clientsFaker);

            modelBuilder.Entity<RoomType>().HasData(roomTypesFaker);
            modelBuilder.Entity<Room>().HasData(roomsFaker);
            modelBuilder.Entity<Client>().HasData(clientsFaker);
            modelBuilder.Entity<Reservation>().HasData(reservationsFaker);

        }

        public bool IsInMemoryDatabase()
        {
            return Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
        }
    }

}
