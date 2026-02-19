using System.ComponentModel.DataAnnotations;
using TaskTracker.TasksService.Domain.Entities;

namespace TaskTracker.TasksService.Contracts;

public sealed record TaskItemDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public TaskPriority Priority { get; init; }
    public bool IsCompleted { get; init; }
    public DateOnly? DueDate { get; init; }
}

public sealed record CreateTaskRequest(
    [Required]
    [MinLength(1)]
    string Title,
    string? Description,
    TaskPriority Priority,
    DateOnly? DueDate
);

public sealed record UpdateTaskRequest(
    [Required]
    [MinLength(1)]
    string Title,
    string? Description,
    TaskPriority Priority,
    DateOnly? DueDate,
    bool IsCompleted
);
