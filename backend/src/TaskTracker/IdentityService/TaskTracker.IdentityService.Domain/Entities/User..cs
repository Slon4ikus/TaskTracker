namespace TaskTracker.IdentityService.Domain.Entities;

public sealed class User
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
}
