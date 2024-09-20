using Bogus;
using DevHotelAPI.Entities;
using DevHotelAPI.Services;
using DevHotelAPI.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using DevHotelAPI.Dtos;
using Microsoft.AspNetCore.Identity;

namespace DevHotelAPI.Contexts
{
    public class HotelDevContext(DbContextOptions<HotelDevContext> options, IBogusRepository bogusRepo, IHostEnvironment env) : DbContext(options)
    {
        private readonly IHostEnvironment _env = env;

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        private IBogusRepository BogusRepo { get; set; } = bogusRepo;
        public void DbInitialize(ModelBuilder modelBuilder)
        {
            var roomTypesFaker = BogusRepo.GenerateRoomTypes();
            var roomsFaker = BogusRepo.GenerateRooms();
            var customersFaker = BogusRepo.GenerateCustomers();
            var reservationsFaker = BogusRepo.GenerateReservations();

            modelBuilder.Entity<RoomType>().HasData(roomTypesFaker);
            modelBuilder.Entity<Room>().HasData(roomsFaker);
            modelBuilder.Entity<Customer>().HasData(customersFaker);
            modelBuilder.Entity<Reservation>().HasData(reservationsFaker);

        }

        public bool IsInMemoryDatabase()
        {
            return Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(p => p.Email).IsRequired();
                entity.HasIndex(p => p.Email).IsUnique();
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasOne(e => e.Customer).WithMany(e => e.Reservations)
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
                entity.HasIndex(e => e.Description).IsUnique();
                entity.Property(e => e.TotalNumber).IsRequired();
                entity.Property(e => e.Capacity).IsRequired();
                entity.HasKey(e => e.Id);
            });

            if (_env.IsDevelopment() || _env.IsStaging())
                DbInitialize(modelBuilder);

            base.OnModelCreating(modelBuilder);

        }
    }

}
