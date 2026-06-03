
using AndrezOG.Infrastructure.ContextDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AndrezOG.Infrastructure.Repository;
using AndrezOG.Application;
using AndrezOG.Application.Iservices;
using AndrezOG.Domain.Irepository;
using DotNetEnv;


Env.Load("../.env.local");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var connectionString = string.Format(
    "Host={0};Port={1};Database={2};Username={3};Password={4}",
    Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost",
    Environment.GetEnvironmentVariable("DB_PORT") ?? "5432",
    Environment.GetEnvironmentVariable("DB_NAME") ?? "andrezog",
    Environment.GetEnvironmentVariable("DB_USER") ?? "postgres",
    Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "postgres"
);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repositorios
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// Servicios

builder.Services.AddScoped<IAuthService, AuthService>();

// configuracion JWT
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
    ?? builder.Configuration["Jwt:Key"]
    ?? "ClaveDeDesarrolloLocal-SoloParaFallback-2025!";


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
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "AndrezOG-App",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();