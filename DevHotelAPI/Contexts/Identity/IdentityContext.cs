using DevHotelAPI.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevHotelAPI.Contexts.Identity
{
    public class IdentityContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
    {
        private IBogusRepository _bogusRepo { get; set; }
        private readonly IHostEnvironment _env;

        public IdentityContext(DbContextOptions<IdentityContext> options, IBogusRepository bogusRepo, IHostEnvironment env) : base(options)
        {
            _bogusRepo = bogusRepo;
            _env = env;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (_env.IsDevelopment() || _env.IsStaging())
                DbInitialize(modelBuilder);
            
            base.OnModelCreating(modelBuilder);

        }

        public void DbInitialize(ModelBuilder modelBuilder)
        {
            var usersFaker = _bogusRepo.GenerateUsers();
            var rolesFaker = _bogusRepo.GenerateRoles();
            var usersRolesFaker = _bogusRepo.AssignRolesToFakeUsers();

            modelBuilder.Entity<IdentityUser<Guid>>().HasData(usersFaker);
            modelBuilder.Entity<IdentityRole<Guid>>().HasData(rolesFaker);
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(usersRolesFaker);
        }
    }
}

