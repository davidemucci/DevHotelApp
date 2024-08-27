using DevHotelAPI.Entities;
using Microsoft.EntityFrameworkCore;

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
                entity.HasOne(e => e.Client).WithMany(e => e.Reservations);
                entity.Property(e => e.From).IsRequired();
                entity.Property(e => e.To).IsRequired();
                entity.Property(e => e.To).IsRequired();
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Number);
                entity.HasOne(e => e.Type);
            });

            modelBuilder.Entity<RoomType>().HasKey(e => e.Id);
        }
    }
}
