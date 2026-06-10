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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(); // Habilitar CORS

app.UseDefaultFiles(); // Permite buscar index.html por defecto
app.UseStaticFiles(); // Habilitar archivos estáticos para las imágenes

app.UseAuthentication(); // Habilitar Autenticación
app.UseAuthorization(); // Habilitar Autorización

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
