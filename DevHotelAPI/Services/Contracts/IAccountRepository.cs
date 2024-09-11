using Microsoft.AspNetCore.Identity;

namespace DevHotelAPI.Services.Contracts
{
    public interface IAccountRepository
    {
        public string? GenerateToken(string userName, List<string> roles);
    }
}
