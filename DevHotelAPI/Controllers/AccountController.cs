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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError("", "Invalid email or password");
                return BadRequest(ModelState);
            }
            else
            {
                var userRoles = (List<string>)await _userManager.GetRolesAsync(user);
                var token = _repo.GenerateToken(user.UserName!, userRoles);
                return Ok(new { token });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<IdentityUser<Guid>>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AddOrUpdateCustomerModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existedUser = await _userManager.FindByNameAsync(model.UserName);
            if (existedUser != null)
            {
                ModelState.AddModelError("", "User name is already taken");
                return BadRequest(ModelState);
            }

            var user = new IdentityUser<Guid>()
            {
                UserName = model.UserName,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
                return Ok(_repo.GenerateToken(model.UserName, new List<string>() { "Consumer" }));
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
                return BadRequest(ModelState); 

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
