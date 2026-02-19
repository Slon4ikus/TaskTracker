// File: backend/src/TaskTracker.TasksService.Tests/TasksApiTests.cs

using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using TaskTracker.TasksService.Contracts;
using TaskTracker.TasksService.Domain.Entities;
using Xunit;

namespace TaskTracker.TasksService.Tests;

public sealed class TasksApiTests
{
    [Fact]
    public async Task CreateTask_Then_GetById_ShouldReturnTask()
    {
        using var factory = new TasksServiceWebAppFactory();
        using var client = factory.CreateClient();

        var userId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        SetUser(client, userId);

        var createRequest = new CreateTaskRequest(
            Title: "Test task",
            Description: "Test description",
            Priority: TaskPriority.Medium,
            DueDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        );

        var createResponse = await client.PostAsJsonAsync("/tasks", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<CreatedResponse>();
        created.Should().NotBeNull();

        var getResponse = await client.GetAsync($"/tasks/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateTask_ShouldPersistChanges()
    {
        using var factory = new TasksServiceWebAppFactory();
        using var client = factory.CreateClient();

        var userId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        SetUser(client, userId);

        var createResponse = await client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskRequest(
                "Original",
                "Original desc",
                TaskPriority.Low,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
            ));

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<CreatedResponse>();
        created.Should().NotBeNull();

        var updateRequest = new UpdateTaskRequest(
            "Updated",
            "Updated desc",
            TaskPriority.High,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            true
        );

        var updateResponse = await client.PutAsJsonAsync($"/tasks/{created!.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/tasks/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var task = await getResponse.Content.ReadFromJsonAsync<TaskItemDto>();
        task.Should().NotBeNull();
        task!.Title.Should().Be("Updated");
        task.IsCompleted.Should().BeTrue();
        task.Priority.Should().Be(TaskPriority.High);
    }

    [Fact]
    public async Task CannotAccessTask_OfAnotherUser_ShouldReturn404()
    {
        using var factory = new TasksServiceWebAppFactory();
        using var client = factory.CreateClient();

        var userA = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userB = Guid.Parse("22222222-2222-2222-2222-222222222222");

        SetUser(client, userA);

        var createResponse = await client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskRequest("Private", null, TaskPriority.Medium, null));

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<CreatedResponse>();
        created.Should().NotBeNull();

        SetUser(client, userB);

        var getResponse = await client.GetAsync($"/tasks/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTask_ShouldRemoveTask()
    {
        using var factory = new TasksServiceWebAppFactory();
        using var client = factory.CreateClient();

        var userId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        SetUser(client, userId);

        var createResponse = await client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskRequest("To delete", null, TaskPriority.Low, null));

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<CreatedResponse>();
        created.Should().NotBeNull();

        var deleteResponse = await client.DeleteAsync($"/tasks/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/tasks/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTask_WithoutTitle_ShouldReturnBadRequest()
    {
        using var factory = new TasksServiceWebAppFactory();
        using var client = factory.CreateClient();

        var userId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        SetUser(client, userId);

        var response = await client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskRequest("", null, TaskPriority.Low, null));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetUserTasks_ShouldReturnOnlyUserTasks()
    {
        using var factory = new TasksServiceWebAppFactory();
        using var client = factory.CreateClient();

        var userA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var userB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        SetUser(client, userA);
        await client.PostAsJsonAsync("/tasks",
            new CreateTaskRequest("Task A", null, TaskPriority.Low, null));

        SetUser(client, userB);
        await client.PostAsJsonAsync("/tasks",
            new CreateTaskRequest("Task B", null, TaskPriority.Low, null));

        SetUser(client, userA);

        var response = await client.GetAsync("/tasks");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tasks = await response.Content.ReadFromJsonAsync<List<TaskItemDto>>();
        tasks.Should().NotBeNull();
        tasks!.Should().HaveCount(1);
        tasks[0].Title.Should().Be("Task A");
    }

    private static void SetUser(HttpClient client, Guid userId)
    {
        client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId.ToString());
    }

    private sealed record CreatedResponse(Guid Id);
}
