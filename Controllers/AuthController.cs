using AgroScan.API.DTOs;
using AgroScan.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgroScan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(CreateUserDto createUserDto)
        {
            try
            {
                var user = await _userService.CreateUserAsync(createUserDto);
                var loginDto = new LoginDto
                {
                    Email = createUserDto.Email,
                    Password = createUserDto.Password
                };
                var authResponse = await _userService.LoginAsync(loginDto);
                return Ok(authResponse);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            var authResponse = await _userService.LoginAsync(loginDto);
            if (authResponse == null)
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(authResponse);
        }
    }
}
