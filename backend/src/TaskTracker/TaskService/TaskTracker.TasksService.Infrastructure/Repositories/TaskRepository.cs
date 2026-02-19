using Microsoft.EntityFrameworkCore;
using TaskTracker.TasksService.Application.Abstractions;
using TaskTracker.TasksService.Domain.Entities;
using TaskTracker.TasksService.Infrastructure.Persistence;

namespace TaskTracker.TasksService.Infrastructure.Repositories;

public sealed class TaskRepository : ITaskRepository
{
    private readonly TasksDbContext _db;

    public TaskRepository(TasksDbContext db)
    {
        _db = db;
    }

    public Task<List<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return _db.Tasks
            .AsNoTracking()
            .Where(x => x.OwnerUserId == userId)
            .OrderByDescending(x => x.DueDateUtc)
            .ToListAsync(ct);
    }

    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return _db.Tasks
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task AddAsync(TaskItem task, CancellationToken ct)
    {
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken ct)
    {
        _db.Tasks.Update(task);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(TaskItem task, CancellationToken ct)
    {
        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync(ct);
    }
}
