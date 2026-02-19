using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskTracker.TasksService.Application.Abstractions;
using TaskTracker.TasksService.Domain.Entities;
using TaskTracker.TasksService.Contracts;

namespace TaskTracker.TasksService.Api.Controllers;

[ApiController]
[Route("tasks")]
[Authorize]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskRepository _repo;

    public TasksController(ITaskRepository repo)
    {
        _repo = repo;
    }

    private Guid GetUserId()
    {
        var value =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (value == null || !Guid.TryParse(value, out var userId))
            throw new UnauthorizedAccessException();

        return userId;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserTaskList(CancellationToken ct)
    {
        var userId = GetUserId();
        var tasks = await _repo.GetByUserIdAsync(userId, ct);
        var result = tasks.Select(t => new TaskItemDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Priority = t.Priority,
            IsCompleted = t.IsCompleted,
            DueDate = t.DueDateUtc
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var userId = GetUserId();

        var task = await _repo.GetByIdAsync(id, ct);
        if (task == null || task.OwnerUserId != userId)
            return NotFound();

        var result = new TaskItemDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            IsCompleted = task.IsCompleted,
            DueDate = task.DueDateUtc
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest req, CancellationToken ct)
    {
        var userId = GetUserId();

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            OwnerUserId = userId,
            Title = req.Title,
            Description = req.Description,
            Priority = req.Priority,
            DueDateUtc = req.DueDate,
            IsCompleted = false,
        };

        await _repo.AddAsync(task, ct);

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, new { id = task.Id });

    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest req, CancellationToken ct)
    {
        var userId = GetUserId();

        var task = await _repo.GetByIdAsync(id, ct);
        if (task == null || task.OwnerUserId != userId)
            return NotFound();

        task.Title = req.Title;
        task.Description = req.Description;
        task.Priority = req.Priority;
        task.DueDateUtc = req.DueDate;
        task.IsCompleted = req.IsCompleted;

        await _repo.UpdateAsync(task, ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var userId = GetUserId();

        var task = await _repo.GetByIdAsync(id, ct);
        if (task == null || task.OwnerUserId != userId)
            return NotFound();

        await _repo.DeleteAsync(task, ct);

        return NoContent();
    }
}
