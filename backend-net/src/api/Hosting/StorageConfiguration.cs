namespace AndrezOG.Api.Hosting;

using AndrezOG.Shared.StorageService;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class StorageConfiguration
{
    public static IServiceCollection AddFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StorageOptions>(configuration.GetSection("Storage"));

        var storageProvider = configuration.GetValue<string>("Storage:Provider") ?? "Local";

        if (storageProvider == "GoogleCloud")
        {
            services.AddSingleton(StorageClient.Create());
            services.AddScoped<IFileStorageService, GoogleCloudStorageService>();
        }
        else
        {
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<LocalFileStorageService>();
        }

        return services;
    }
}