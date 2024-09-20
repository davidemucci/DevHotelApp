using DevHotelAPI.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevHotelAPI.Contexts.Identity
{
    public class IdentityContext(DbContextOptions<IdentityContext> options, IBogusRepository bogusRepo, IHostEnvironment env) : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>(options)
    {
        private IBogusRepository BogusRepo { get; set; } = bogusRepo;
        private readonly IHostEnvironment _env = env;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (_env.IsDevelopment() || _env.IsStaging())
                DbInitialize(modelBuilder);
            
            base.OnModelCreating(modelBuilder);

        }

        public void DbInitialize(ModelBuilder modelBuilder)
        {
            var usersFaker = BogusRepo.GenerateUsers();
            var rolesFaker = BogusRepo.GenerateRoles();
            var usersRolesFaker = BogusRepo.AssignRolesToFakeUsers();

            modelBuilder.Entity<IdentityUser<Guid>>().HasData(usersFaker);
            modelBuilder.Entity<IdentityRole<Guid>>().HasData(rolesFaker);
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(usersRolesFaker);
        }
    }
}

