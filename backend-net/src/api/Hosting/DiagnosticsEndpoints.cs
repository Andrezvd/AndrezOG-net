namespace AndrezOG.Api.Hosting;

using AndrezOG.Infrastructure.ContextDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public static class DiagnosticsEndpoints
{
    public static IEndpointConventionBuilder MapHealthEndpoint(this WebApplication app)
    {
        return app.MapGet("/health", async (AppDbContext db) =>
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
                    result["database_error"] = "CanConnectAsync returned false";
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
    }
}