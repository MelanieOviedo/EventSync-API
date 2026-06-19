using EventSync_API.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSync_API.Data
{
    public static class DatabaseSeeder
    {
        
        public static async Task SeedAsync(AppDbContext context)
        {
            
            await context.Database.MigrateAsync();

            var adminUser = new User
            {
                FullName = "Administrador del Sistema",
                Email = "admin@admin.com",
                PasswordHash = "admin1234",
                Role = "Admin",
                FcmToken = null
            };

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }
    }
}