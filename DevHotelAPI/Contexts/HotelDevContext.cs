using Bogus;
using DevHotelAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DevHotelAPI.Contexts
{
    public class HotelDevContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }

        public HotelDevContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
                entity.HasOne(e => e.Type).WithOne()
                .HasForeignKey<Room>(r => r.RoomTypeId)
                .IsRequired();
                
            });

            modelBuilder.Entity<RoomType>().HasKey(e => e.Id);

            DbInitialize(modelBuilder);

        }

        private void DbInitialize(ModelBuilder modelBuilder)
        {
            var id = 1;
            var descRoomTypes = new List<string>(){ "Room", "TwinRoom",  "Triple", "Suite" };
            var roomTypes = new Faker<RoomType>()
                .RuleFor(r => r.Id, f => id++)
                .RuleFor(r => r.Description, f => f.PickRandom(descRoomTypes))
                .RuleFor(r => r.TotalNumber, f => f.Random.Int(1, 50));
            var roomNumber = 100;

            var room = new Faker<Room>()
                  .RuleFor(r => r.Number, f => roomNumber++)
                  .RuleFor(r => r.RoomTypeId, f => id);

            var reservation = new Faker<Reservation>()
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.RoomNumber, f => roomNumber++);

            var client = new Faker<Client>()
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.Email, f => f.Internet.Email())
                .RuleFor(r => r.Password, f => f.Internet.Password())
                .RuleFor(r => r.Address, f => f.Address.StreetAddress())
                .RuleFor(r => r.Reservations, (f, c) =>
                {
                    reservation.RuleFor(r => r.ClientId, _ => c.Id);
                    reservation.GenerateBetween(1, 5);
                    return null;
                });

            modelBuilder.Entity<RoomType>().HasData(roomTypes.GenerateBetween(2, 3));
            modelBuilder.Entity<Room>().HasData(room.GenerateBetween(2, 12));
            modelBuilder.Entity<Client>().HasData(client.GenerateBetween(1, 3));
            modelBuilder.Entity<Reservation>().HasData(reservation);
        }
    }
}
