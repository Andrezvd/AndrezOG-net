namespace AndrezOG.Api.Hosting;

using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

public static class JsonOptionsConfiguration
{
    public static IServiceCollection AddControllersWithJsonOptions(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        return services;
    }
}