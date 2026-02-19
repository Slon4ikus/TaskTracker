using Microsoft.EntityFrameworkCore;
using TaskTracker.IdentityService.Application.Abstractions;
using TaskTracker.IdentityService.Domain.Entities;
using TaskTracker.IdentityService.Infrastructure.Persistence;

namespace TaskTracker.IdentityService.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _db;

    public UserRepository(IdentityDbContext db) => _db = db;

    public Task<User?> FindByUserNameAsync(string userName, CancellationToken ct) =>
        _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == userName, ct);
}
