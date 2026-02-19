using TaskTracker.TasksService.Domain.Entities;

namespace TaskTracker.TasksService.Application.Abstractions;

public interface ITaskRepository
{
    Task<List<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken ct);

    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct);

    Task AddAsync(TaskItem task, CancellationToken ct);

    Task UpdateAsync(TaskItem task, CancellationToken ct);

    Task DeleteAsync(TaskItem task, CancellationToken ct);
}
