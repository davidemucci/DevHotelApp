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
            DbInitialize(modelBuilder);
            base.OnModelCreating(modelBuilder);

        }

        public void DbInitialize(ModelBuilder modelBuilder)
        {
            var usersFaker = _bogusRepo.GenerateUsers();
            var rolesFaker = _bogusRepo.GenerateRoles();
            var usersRolesFaker = _bogusRepo.AssignRolesToFakeUsers(rolesFaker, usersFaker);

            modelBuilder.Entity<IdentityUser<Guid>>().HasData(usersFaker);
            modelBuilder.Entity<IdentityRole<Guid>>().HasData(rolesFaker);
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(usersRolesFaker);
        }
    }
}

