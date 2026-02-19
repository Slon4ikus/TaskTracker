using TaskTracker.IdentityService.Domain.Entities;

namespace TaskTracker.IdentityService.Application.Abstractions;

public interface IUserRepository
{
    Task<User?> FindByUserNameAsync(string userName, CancellationToken ct);
}
