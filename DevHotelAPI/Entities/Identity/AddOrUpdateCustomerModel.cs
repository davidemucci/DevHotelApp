using System.ComponentModel.DataAnnotations;

namespace DevHotelAPI.Entities.Identity
{
    public class AddOrUpdateCustomerModel
    {
        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; } = string.Empty;
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
