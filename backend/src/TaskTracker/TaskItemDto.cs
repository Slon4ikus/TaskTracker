namespace TaskTracker.TasksService.Contracts;

public sealed record TaskItemDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public int Priority { get; init; }
    public bool IsCompleted { get; init; }

    public DateTime? DueDate { get; init; }
}
