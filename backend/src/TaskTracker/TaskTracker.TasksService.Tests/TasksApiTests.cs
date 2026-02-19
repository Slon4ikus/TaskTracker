using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using TaskTracker.TasksService.Api.Controllers;
using TaskTracker.TasksService.Contracts;
using TaskTracker.TasksService.Domain;
using TaskTracker.TasksService.Domain.Entities;
using Xunit;

namespace TaskTracker.TasksService.Tests;

public sealed class TasksApiTests : IClassFixture<TasksServiceWebAppFactory>
{
    private readonly HttpClient _client;

    public TasksApiTests(TasksServiceWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTask_Then_GetById_ShouldReturnTask()
    {
        var userId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Add(
            TestAuthHandler.UserIdHeader,
            userId.ToString());

        var createRequest = new CreateTaskRequest(
            Title: "Test task",
            Description: "Test description",
            Priority: TaskPriority.Medium,
            DueDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        );

        var createResponse = await _client.PostAsJsonAsync(
            "/tasks",
            createRequest);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content
            .ReadFromJsonAsync<CreatedResponse>();

        created.Should().NotBeNull();

        var getResponse = await _client.GetAsync(
            $"/tasks/{created!.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateTask_ShouldPersistChanges()
    {
        var userId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Add(
            TestAuthHandler.UserIdHeader,
            userId.ToString());

        var createRequest = new CreateTaskRequest(
            "Original",
            "Original desc",
            TaskPriority.Low,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        );

        var createResponse = await _client.PostAsJsonAsync(
            "/tasks",
            createRequest);

        var created = await createResponse.Content
            .ReadFromJsonAsync<CreatedResponse>();

        var updateRequest = new UpdateTaskRequest(
            "Updated",
            "Updated desc",
            TaskPriority.High,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            true
        );

        var updateResponse = await _client.PutAsJsonAsync(
            $"/tasks/{created!.Id}",
            updateRequest);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync(
            $"/tasks/{created.Id}");

        var task = await getResponse.Content
            .ReadFromJsonAsync<TaskItemDto>();

        task!.Title.Should().Be("Updated");
        task.IsCompleted.Should().BeTrue();
        task.Priority.Should().Be(TaskPriority.High);
    }

    [Fact]
    public async Task CannotAccessTask_OfAnotherUser_ShouldReturn404()
    {
        var userA = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userB = Guid.Parse("22222222-2222-2222-2222-222222222222");

        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Add(
            TestAuthHandler.UserIdHeader,
            userA.ToString());

        var createRequest = new CreateTaskRequest(
            "Private",
            null,
            TaskPriority.Medium,
            null
        );

        var createResponse = await _client.PostAsJsonAsync(
            "/tasks",
            createRequest);

        var created = await createResponse.Content
            .ReadFromJsonAsync<CreatedResponse>();

        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Add(
            TestAuthHandler.UserIdHeader,
            userB.ToString());

        var getResponse = await _client.GetAsync(
            $"/tasks/{created!.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTask_ShouldRemoveTask()
    {
        var userId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Add(
            TestAuthHandler.UserIdHeader,
            userId.ToString());

        var createRequest = new CreateTaskRequest(
            "To delete",
            null,
            TaskPriority.Low,
            null
        );

        var createResponse = await _client.PostAsJsonAsync(
            "/tasks",
            createRequest);

        var created = await createResponse.Content
            .ReadFromJsonAsync<CreatedResponse>();

        var deleteResponse = await _client.DeleteAsync(
            $"/tasks/{created!.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync(
            $"/tasks/{created.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTask_WithoutTitle_ShouldReturnBadRequest()
    {
        var userId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Add(
            TestAuthHandler.UserIdHeader,
            userId.ToString());

        var createRequest = new CreateTaskRequest(
            "",
            null,
            TaskPriority.Low,
            null
        );

        var response = await _client.PostAsJsonAsync(
            "/tasks",
            createRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetUserTasks_ShouldReturnOnlyUserTasks()
    {
        var userA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var userB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        // Create task for user A
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Add(
            TestAuthHandler.UserIdHeader,
            userA.ToString());

        await _client.PostAsJsonAsync("/tasks",
            new CreateTaskRequest("Task A", null, TaskPriority.Low, null));

        // Create task for user B
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Add(
            TestAuthHandler.UserIdHeader,
            userB.ToString());

        await _client.PostAsJsonAsync("/tasks",
            new CreateTaskRequest("Task B", null, TaskPriority.Low, null));

        // Read tasks for user A
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        _client.DefaultRequestHeaders.Add(
            TestAuthHandler.UserIdHeader,
            userA.ToString());

        var response = await _client.GetAsync("/tasks");

        var tasks = await response.Content
            .ReadFromJsonAsync<List<TaskItemDto>>();

        tasks.Should().HaveCount(1);
        tasks![0].Title.Should().Be("Task A");
    }

    private sealed record CreatedResponse(Guid Id);
}
