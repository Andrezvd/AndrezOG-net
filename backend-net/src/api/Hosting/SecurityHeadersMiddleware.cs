namespace AndrezOG.Api.Hosting;

using Microsoft.AspNetCore.Builder;

public static class SecurityHeadersMiddleware
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
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

        return app;
    }
}