using Microsoft.AspNetCore.Mvc;
using EventSync_API.DTOs;
using EventSync_API.Services;

namespace EventSync_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserResponse>> Login([FromBody] LoginRequest loginRequest)
        {
            var response = await _authService.LoginAsync(loginRequest);

            if (response == null)
            {
                return Unauthorized(new { message = "Credenciales incorrectas" });
            }

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest registerRequest)
        {
            var response = await _authService.RegisterAsync(registerRequest);

            if (response == null)
            {
                return BadRequest(new { message = "El usuario ya existe" });
            }

            return CreatedAtAction(nameof(Login), new { id = response.Id }, response);
        }
    }
}