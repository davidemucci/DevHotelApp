using DevHotelAPI.Entities.Identity;
using DevHotelAPI.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevHotelAPI.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController(UserManager<IdentityUser<Guid>> _userManager, IAccountRepository _repo) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError("", "Invalid username or password");
                return BadRequest(ModelState);
            }
            else
            {
                var token = _repo.GenerateToken(model.UserName);
                return Ok(new { token });
            }
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
            {
                var token = _repo.GenerateToken(model.UserName);
                return Ok(token);
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
    }
}
