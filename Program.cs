using Microsoft.EntityFrameworkCore;
using EventSync_API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registro de Repositorios y Servicios
builder.Services.AddScoped<EventSync_API.Repositories.IUserRepository, EventSync_API.Repositories.UserRepository>();
builder.Services.AddScoped<EventSync_API.Services.IAuthService, EventSync_API.Services.AuthService>();
builder.Services.AddScoped<EventSync_API.Repositories.IEventRepository, EventSync_API.Repositories.EventRepository>();
builder.Services.AddScoped<EventSync_API.Services.IEventService, EventSync_API.Services.EventService>();
builder.Services.AddScoped<EventSync_API.Repositories.IBookingRepository, EventSync_API.Repositories.BookingRepository>();
builder.Services.AddScoped<EventSync_API.Services.IBookingService, EventSync_API.Services.BookingService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Obtenemos la instancia de DbContext
        var context = services.GetRequiredService<AppDbContext>();

        // Ejecutamos el seeder
        await DatabaseSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al ejecutar el seeder de la base de datos.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// Archivos Estáticos (Esto expone la carpeta wwwroot)
app.UseDefaultFiles();
app.UseStaticFiles();

// Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

// Mapeo de Controladores
app.MapControllers();

app.Run();