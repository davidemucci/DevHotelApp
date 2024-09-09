using DevHotelAPI.Entities;
using DevHotelAPI.Entities.Identity;
using DevHotelAPI.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevHotelAPI.Contexts.Identity
{
    public class IdentityContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
    {
        private IBogusRepository _bogusRepo { get; set; }

        public IdentityContext(DbContextOptions<IdentityContext> options, IBogusRepository bogusRepo) : base(options)
        {
            _bogusRepo = bogusRepo;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _bogusRepo.GenerateUsers();

            DbInitialize(modelBuilder);
            base.OnModelCreating(modelBuilder);

        }

        public void DbInitialize(ModelBuilder modelBuilder)
        {
            var usersFaker = _bogusRepo.GenerateUsers();
            modelBuilder.Entity<IdentityUser<Guid>>().HasData(usersFaker);

        }


    }
}
