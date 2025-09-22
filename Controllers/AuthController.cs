using AgroScan.API.DTOs;
using AgroScan.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgroScan.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : BaseController
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
                var validationResult = ValidateModelState();
                if (validationResult != null) return validationResult;

                var user = await _userService.CreateUserAsync(createUserDto);
                var loginDto = new LoginDto
                {
                    Email = createUserDto.Email,
                    Password = createUserDto.Password
                };
                var authResponse = await _userService.LoginAsync(loginDto);
                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                return HandleServiceException(ex);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            try
            {
                var validationResult = ValidateModelState();
                if (validationResult != null) return validationResult;

                var authResponse = await _userService.LoginAsync(loginDto);
                if (authResponse == null)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                return HandleServiceException(ex);
            }
        }
    }
}
