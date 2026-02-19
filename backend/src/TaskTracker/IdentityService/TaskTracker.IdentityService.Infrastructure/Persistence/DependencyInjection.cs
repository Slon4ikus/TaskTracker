using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskTracker.IdentityService.Application.Abstractions;
using TaskTracker.IdentityService.Infrastructure.Repositories;

namespace TaskTracker.IdentityService.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(o =>
            o.UseSqlite(configuration.GetConnectionString("IdentityDb")));

        services.AddScoped<DbInitializer>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
