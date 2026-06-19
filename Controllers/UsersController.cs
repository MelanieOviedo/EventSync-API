using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EventSync_API.Repositories;
using EventSync_API.Models;
using EventSync_API.DTOs;
using EventSync_API.Mappers;

namespace EventSync_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<UserResponse>> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null) return NotFound(new { message = "Usuario no encontrado" });

            return Ok(UserMapper.ToUserResponse(user));
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null) return NotFound(new { message = "Usuario no encontrado" });

            // Nota: Al igual que en el login, comparamos texto plano por ahora
            if (user.PasswordHash != request.CurrentPassword)
                return BadRequest(new { message = "La contraseña actual es incorrecta" });

            user.PasswordHash = request.NewPassword;
            await _userRepository.UpdateUserAsync(user);

            return Ok(new { message = "Contraseña actualizada exitosamente" });
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserCreateDto userDto) 
        {
            var user = new User
            {
                FullName = userDto.FullName,
                Email = userDto.Email,
                PasswordHash = userDto.PasswordHash,
                Role = userDto.Role
            };

            await _userRepository.CreateUserAsync(user);

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }

        [HttpPut("fcm-token")]
        public async Task<IActionResult> UpdateFcmToken([FromBody] FcmTokenRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Usuario no válido o sesión expirada." });
            }

            if (string.IsNullOrEmpty(request.FcmToken))
            {
                return BadRequest(new { message = "El token FCM es requerido." });
            }

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            if (user.FcmToken != request.FcmToken)
            {
                user.FcmToken = request.FcmToken;
                await _userRepository.UpdateUserAsync(user);
            }

            return Ok(new { message = "Token FCM actualizado con éxito en el servidor." });
        }
    }
}