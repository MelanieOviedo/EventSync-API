using EventSync_API.Models;

namespace EventSync_API.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAndPasswordAsync(string email, string password);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
    }
}