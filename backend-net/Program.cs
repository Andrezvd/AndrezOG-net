
using AndrezOG.Infrastructure.ContextDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AndrezOG.Infrastructure.Repository;
using AndrezOG.Application;
using AndrezOG.Application.Iservices;
using AndrezOG.Domain.Irepository;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
// CORS: permitir requests desde el frontend React en desarrollo (Vite: localhost:5173)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repositorios
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();

// Servicios
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();

// configuracion JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ClaveDeDesarrolloLocal-SoloParaFallback-2025!";


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "AndrezOG",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "AndrezOG-Client",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
// Permitir react mediante cors
app.UseCors("AllowReactDev");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();