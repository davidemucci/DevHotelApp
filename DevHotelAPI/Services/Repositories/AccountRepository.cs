using DevHotelAPI.Services.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DevHotelAPI.Services.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public AccountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration _configuration { get; set; }

        public string? GenerateToken(string email, List<string> roles)
        {
            var secret = _configuration["JwtConfig:Secret"];
            var issuer = _configuration["JwtConfig:ValidIssuer"];
            var audience = _configuration["JwtConfig:ValidAudiences"];

            if (secret is null || issuer is null || audience is null)
                throw new ApplicationException("Jwt is not set in the configuration");

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.Ticks.ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Name, email)
            };

            if (roles != null && roles.Any())
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, email));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
            };


            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return token;
        }
    }
}
