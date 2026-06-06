using EventSync_API.DTOs;
using EventSync_API.Repositories;
using EventSync_API.Mappers;
using EventSync_API.Models;

namespace EventSync_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponse?> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userRepository.GetUserByEmailAndPasswordAsync(loginRequest.Email, loginRequest.Password);
            
            if (user == null)
            {
                return null;
            }

            return UserMapper.ToUserResponse(user);
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
                FullName = registerRequest.Name,
                Email = registerRequest.Email,
                PasswordHash = registerRequest.Password, // Guardando temporalmente en texto plano
                Role = "Customer"
            };

            var createdUser = await _userRepository.CreateUserAsync(newUser);
            return UserMapper.ToUserResponse(createdUser);
        }
    }
}