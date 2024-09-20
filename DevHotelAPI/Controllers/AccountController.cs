using DevHotelAPI.Contexts.Identity;
using DevHotelAPI.Entities.Identity;
using DevHotelAPI.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHotelAPI.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController(UserManager<IdentityUser<Guid>> _userManager, IAccountRepository _repo, IdentityContext _context) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return BadRequest("Invalid email or password.");
            }
            else
            {
                var userRoles = (List<string>)await _userManager.GetRolesAsync(user);
                var token = _repo.GenerateToken(user.UserName!, userRoles);
                return Ok(new { token });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AddOrUpdateCustomerModel model)
        {
            var existedUser = await _userManager.FindByNameAsync(model.UserName);
            var existedUserEmail = await _userManager.FindByNameAsync(model.Email);
            if (existedUser != null || existedUserEmail != null)
            {
                return BadRequest("User name or email is already taken");
            }

            var user = new IdentityUser<Guid>()
            {
                UserName = model.UserName,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            

            if (result.Succeeded)
            {
                 await _repo.CreateCostumer(user);
                return Ok(_repo.GenerateToken(model.UserName, new List<string>() { "Consumer" }));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole(string username, string role)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
                return BadRequest($"User with {username} not found."); 

            try
            {
                await _userManager.AddToRoleAsync(user, role);
                return Ok();
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
