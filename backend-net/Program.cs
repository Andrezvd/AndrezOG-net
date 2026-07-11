
using AndrezOG.Infrastructure.ContextDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using AndrezOG.Infrastructure.Repository;
using AndrezOG.Application;
using AndrezOG.Application.Iservices;
using AndrezOG.Domain.Irepository;
using AndrezOG.Domain.Model.Skills;
using AndrezOG.Shared.StorageService;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// CORS: permitir requests desde frontends en desarrollo y producción
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendDev", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "http://localhost:4200",
            "https://andrezog.com",
            "https://www.andrezog.com"
        )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// File Storage: Local (desarrollo) o Google Cloud Storage (produccion)
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));
var storageProvider = builder.Configuration.GetValue<string>("Storage:Provider") ?? "Local";
if (storageProvider == "GoogleCloud")
{
    builder.Services.AddSingleton(Google.Cloud.Storage.V1.StorageClient.Create());
    builder.Services.AddScoped<IFileStorageService, GoogleCloudStorageService>();
}
else
{
    builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
    builder.Services.AddScoped<LocalFileStorageService>(); // Para compatibilidad directa si es necesario
}

// Límite global de cuerpo de request para evitar payloads maliciosos
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB máximo
});

// Rate Limiter (previene fuerza bruta y abuso)
builder.Services.AddRateLimiter(options =>
{
    // Login: 5 intentos por minuto por IP
    options.AddFixedWindowLimiter("login", config =>
    {
        config.PermitLimit = 5;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 0;
    });

    // Registro: 3 solicitudes por 10 minutos por IP
    options.AddFixedWindowLimiter("register", config =>
    {
        config.PermitLimit = 3;
        config.Window = TimeSpan.FromMinutes(10);
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 0;
    });

    // Refresh token: 5 solicitudes por minuto por IP
    options.AddFixedWindowLimiter("refresh", config =>
    {
        config.PermitLimit = 5;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 0;
    });

    // Upload de archivos: 10 solicitudes por minuto por IP
    options.AddFixedWindowLimiter("upload", config =>
    {
        config.PermitLimit = 10;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 0;
    });

    // Endpoints públicos: 100 solicitudes por minuto por IP
    options.AddFixedWindowLimiter("public", config =>
    {
        config.PermitLimit = 100;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// IHttpContextAccessor para logging de IP
builder.Services.AddHttpContextAccessor();

// Repositorios
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();

// Servicios
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ISkillService, SkillService>();

// configuracion JWT
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("Jwt:Key no configurado.");
}

// Validar longitud mínima del secret JWT (recomendado: al menos 32 bytes para HMAC-SHA256)
if (Encoding.UTF8.GetBytes(jwtKey).Length < 32)
{
    throw new InvalidOperationException("Jwt:Key debe tener al menos 32 caracteres para garantizar seguridad HMAC-SHA256.");
}

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero // Eliminar tolerancia de reloj (default 5 min)
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

// Seguridad HTTP: HSTS solo en produccion (omitir en desarrollo)
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// ─── MIDDLEWARE DIAGNÓSTICO: capturar excepciones no controladas ───
// Temporal: registrar stack trace completo en Logs Explorer.
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Excepción no controlada en {Method} {Path}",
            context.Request.Method, context.Request.Path);
        throw; // Relanzar para que ASP.NET Core maneje el 500
    }
});

// Headers de seguridad
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    
    // CSP básica compatible con Angular SSR:
    // - 'self' para recursos propios
    // - https: para imágenes desde cualquier fuente HTTPS (Google, etc.)
    // - 'unsafe-inline' para estilos (requerido por Angular en algunos casos)
    // - 'unsafe-eval' NO incluido (seguro)
    context.Response.Headers.Append(
        "Content-Security-Policy",
        "default-src 'self'; " +
        "img-src 'self' data: https:; " +
        "style-src 'self' 'unsafe-inline'; " +
        "script-src 'self'; " +
        "font-src 'self' data:; " +
        "connect-src 'self' https:; " +
        "frame-src 'self' https://accounts.google.com;"
    );

    await next();
});

// Servir archivos estáticos desde wwwroot (imágenes de perfil, skills, etc.)
app.UseStaticFiles();

// Permitir frontends mediante CORS
app.UseCors("AllowFrontendDev");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// ─── HEALTH DIAGNÓSTICO: verificar conexión BD ───
// Temporal: probar si la app llega a PostgreSQL.
app.MapGet("/health", async (AppDbContext db) =>
{
    var result = new Dictionary<string, object>
    {
        ["status"] = "ok",
        ["timestamp"] = DateTime.UtcNow.ToString("o")
    };

    try
    {
        var canConnect = await db.Database.CanConnectAsync();
        result["database"] = canConnect;
        if (!canConnect)
        {
            result["database_error"] = "CanConnectAsync returned false";
        }
    }
    catch (Exception ex)
    {
        result["database"] = false;
        result["database_error_type"] = ex.GetType().Name;
        result["database_error_message"] = ex.Message;
        if (ex.InnerException != null)
        {
            result["database_inner_error_type"] = ex.InnerException.GetType().Name;
            result["database_inner_error_message"] = ex.InnerException.Message;
        }
    }

    return Results.Ok(result);
});
app.MapControllers();

// ── Nota: Las migraciones se ejecutan como paso en CI/CD ──
// Ver .github/workflows/deploy.yml -> "Run EF Core migrations"
// para evitar timeouts de startup en Cloud Run.
app.Run();
