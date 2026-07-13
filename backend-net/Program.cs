using AndrezOG.Api.Hosting;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ===== Servicios =====
builder.Services.AddOpenApi();
builder.Services.AddControllersWithJsonOptions();
builder.Services.AddCorsPolicy();
builder.Services.AddDbContext(builder.Configuration);
builder.Services.AddFileStorage(builder.Configuration);
builder.Services.AddRateLimiterPolicies();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);

// Límite global de cuerpo de request para evitar payloads maliciosos
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 5 * 1024 * 1024; // 5MB máximo
});

var app = builder.Build();

// ===== Middleware pipeline =====
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
        throw;
    }
});

app.UseSecurityHeaders();
app.UseStaticFiles();
app.UseCors("AllowFrontendDev");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthEndpoint();
app.MapControllers();

// ── Nota: Las migraciones se ejecutan como paso en CI/CD ──
// Ver .github/workflows/deploy.yml -> "Run EF Core migrations"
// para evitar timeouts de startup en Cloud Run.
app.Run();