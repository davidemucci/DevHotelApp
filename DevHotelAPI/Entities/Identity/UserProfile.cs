using Microsoft.AspNetCore.Identity;

namespace DevHotelAPI.Entities.Identity
{
    public class UserProfile : IdentityUser<Guid>
    {
        public string? Address { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
    }
}
