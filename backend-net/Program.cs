
using AndrezOG.Infrastructure.ContextDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using AndrezOG.Infrastructure.Repository;
using AndrezOG.Infrastructure.Auth;
using AndrezOG.Application;
using AndrezOG.Application.Iservices;
using AndrezOG.Domain.Irepository;
using AndrezOG.Shared.StorageService;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
// CORS: permitir requests desde los frontends React (5173) y Angular (4200) en desarrollo
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// File Storage (singleton: una instancia para toda la app)
var webRootPath = builder.Environment.WebRootPath
    ?? Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
builder.Services.AddSingleton(new FileStorageService(webRootPath));

// Google OAuth
builder.Services.AddSingleton<GoogleAuthService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var http = httpClientFactory.CreateClient();
    var clientId = config["Google:ClientId"] ?? throw new InvalidOperationException("Google:ClientId no configurado");
    var clientSecret = config["Google:ClientSecret"] ?? throw new InvalidOperationException("Google:ClientSecret no configurado");
    return new GoogleAuthService(http, clientId, clientSecret);
});

// Rate Limiter para login (previene fuerza bruta)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("login", config =>
    {
        config.PermitLimit = 5;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Repositorios
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();

// Servicios
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ISkillService, SkillService>();

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
// Permitir frontends mediante CORS
app.UseCors("AllowFrontendDev");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
