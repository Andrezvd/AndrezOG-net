namespace AndrezOG.Api.Hosting;

using AndrezOG.Application;
using AndrezOG.Application.Iservices;
using AndrezOG.Domain.Irepository;
using AndrezOG.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<IStackRepository, StackRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<ISkillService, SkillService>();
        services.AddScoped<IStackService, StackService>();
        services.AddScoped<IProjectService, ProjectService>();

        return services;
    }
}