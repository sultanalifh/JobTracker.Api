using JobTracker.Api.Repositories;
using JobTracker.Api.Security;
using JobTracker.Api.Services;

namespace JobTracker.Api.Utilities;

public static class DependencyInjection
{
    public static IServiceCollection AddJobApplicationRepositories(this IServiceCollection services)
    {
        services.AddScoped<IJobApplicationRepositories, JobApplicationRepositories>();

        return services;
    }

    public static IServiceCollection AddUserRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepositories, UserRepositories>();

        return services;
    }

    public static IServiceCollection AddAuthService(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }

    public static IServiceCollection AddJwtService(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }

    public static IServiceCollection AddPasswordService(this IServiceCollection services)
    {
        services.AddScoped<IPasswordService, PasswordService>();

        return services;
    }

    public static IServiceCollection AddAdminJobApplicationService(this IServiceCollection services)
    {
        services.AddScoped<IAdminJobApplicationService, AdminJobApplicationService>();

        return services;
    }

    public static IServiceCollection AddUserJobApplicationService(this IServiceCollection services)
    {
        services.AddScoped<IUserJobApplicationService, UserJobApplicationService>();

        return services;
    }

    public static IServiceCollection AddCurrentUserService(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }

    public static IServiceCollection AddStatisticsService(this IServiceCollection services)
    {
        services.AddScoped<IStatisticsService, StatisticsService>();

        return services;
    }

    public static IServiceCollection AddUserService(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}