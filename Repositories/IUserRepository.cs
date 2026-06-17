using EventSync_API.Models;

namespace EventSync_API.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAndPasswordAsync(string email, string password);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
    }
}