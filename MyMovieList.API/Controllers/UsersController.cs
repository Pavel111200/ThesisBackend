using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyMovieList.Core.Contracts;
using MyMovieList.Core.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyMovieList.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UsersController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> Register(RegisterUserViewModel user)
        {
            UserViewModel result = new UserViewModel();
            try
            {
                result = await _userService.Register(user);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            result.AccessToken = CreateToken(result);

            return Ok(result);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(LoginUserViewModel user)
        {
            UserViewModel result = new UserViewModel();
            try
            {
                result = await _userService.Login(user);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }

            result.AccessToken = CreateToken(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{userId}")]
        public async Task<IActionResult> ChangeRole(Guid userId, string newRole)
        {
            try
            {
                await _userService.ChangeRole(userId, newRole);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsers();

            return Ok(users);
        }

        [HttpPost("Suggestions")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SendSuggestion(UserSuggestionViewModel model)
        {
            var isSaved = await _userService.Suggestion(model);

            if (!isSaved)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpDelete("Suggestions/{suggestionId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendSuggestion(Guid suggestionId)
        {
            try
            {
                await _userService.DeleteSuggestion(suggestionId);
            }
            catch (ArgumentNullException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        private string CreateToken(UserViewModel user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: credentials);
            
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
