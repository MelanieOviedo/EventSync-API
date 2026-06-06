using Microsoft.EntityFrameworkCore;
using EventSync_API.Data;
using EventSync_API.Models;

namespace EventSync_API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            // Nota: En una aplicación real, las contraseñas deberían estar hasheadas.
            // Aquí se busca directamente como lo solicitaste.
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}