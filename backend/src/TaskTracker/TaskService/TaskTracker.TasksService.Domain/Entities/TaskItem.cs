namespace TaskTracker.TasksService.Domain.Entities;

public enum TaskPriority
{
    Low = 0,
    Medium = 1,
    High = 2
}

public sealed class TaskItem
{
    public Guid Id { get; set; }

    public Guid OwnerUserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public TaskPriority Priority { get; set; }

    public DateOnly? DueDateUtc { get; set; }

    public bool IsCompleted { get; set; }
}
