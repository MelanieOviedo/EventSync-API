using EventSync_API.DTOs;
using EventSync_API.Repositories;
using EventSync_API.Mappers;
using EventSync_API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventSync_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<UserResponse?> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userRepository.GetUserByEmailAndPasswordAsync(loginRequest.Email, loginRequest.Password);
            
            if (user == null)
            {
                return null;
            }

            var response = UserMapper.ToUserResponse(user);
            response.Token = GenerateJwtToken(user);
            response.Message = "Bienvenido de nuevo";
            
            return response;
        }

        public async Task<UserResponse?> RegisterAsync(RegisterRequest registerRequest)
        {
            // Verificar si el usuario ya existe
            var existingUser = await _userRepository.GetUserByEmailAsync(registerRequest.Email);
            if (existingUser != null)
            {
                return null; // O podrías lanzar una excepción personalizada
            }

            var newUser = new User
            {
                FullName = registerRequest.FullName,
                Email = registerRequest.Email,
                PasswordHash = registerRequest.Password, // Guardando temporalmente en texto plano
                Role = "Customer"
            };

            var createdUser = await _userRepository.CreateUserAsync(newUser);
            var response = UserMapper.ToUserResponse(createdUser);
            response.Token = GenerateJwtToken(createdUser);
            response.Message = "Usuario creado exitosamente";

            return response;
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}