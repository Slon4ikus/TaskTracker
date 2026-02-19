using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskTracker.TasksService.Application.Abstractions;
using TaskTracker.TasksService.Infrastructure.Repositories;

namespace TaskTracker.TasksService.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<TasksDbContext>(o =>
            o.UseSqlite(configuration.GetConnectionString("TasksDb")));

        services.AddScoped<ITaskRepository, TaskRepository>();

        return services;
    }
}
