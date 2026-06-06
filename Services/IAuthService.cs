using EventSync_API.DTOs;

namespace EventSync_API.Services
{
    public interface IAuthService
    {
        Task<UserResponse?> LoginAsync(LoginRequest loginRequest);
        Task<UserResponse?> RegisterAsync(RegisterRequest registerRequest);
    }
}