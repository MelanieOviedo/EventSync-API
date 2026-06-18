using EventSync_API.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSync_API.Data
{
    public static class DatabaseSeeder
    {
        // Reemplaza 'AppDbContext' por el nombre real de tu DbContext
        public static async Task SeedAsync(AppDbContext context)
        {
            // Opcional: Asegura que la base de datos y migraciones estén aplicadas
            await context.Database.MigrateAsync();

            // Verificamos si ya existe algún usuario con el rol "Admin"
            if (!await context.Users.AnyAsync(u => u.Role == "Admin"))
            {
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
}