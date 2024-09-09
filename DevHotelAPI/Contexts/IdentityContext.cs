using DevHotelAPI.Entities;
using DevHotelAPI.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevHotelAPI.Contexts
{
    public class IdentityContext : IdentityDbContext<UserProfile, IdentityRole<Guid>, Guid>
    {
        private IConfiguration _configuration;

        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
        }
    }
}
