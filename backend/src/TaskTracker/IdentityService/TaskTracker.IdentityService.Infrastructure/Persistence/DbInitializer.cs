using Microsoft.EntityFrameworkCore;
using TaskTracker.IdentityService.Domain.Entities;

namespace TaskTracker.IdentityService.Infrastructure.Persistence;

public sealed class DbInitializer
{
    private readonly IdentityDbContext _db;

    public DbInitializer(IdentityDbContext db) => _db = db;

    public async Task SeedAsync(CancellationToken ct)
    {
        await _db.Database.MigrateAsync(ct);

        if (await _db.Users.AnyAsync(ct))
            return;

        _db.Users.AddRange(
            new User
            {
                Id = Guid.NewGuid(),
                UserName = "alice",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234")
            },
            new User
            {
                Id = Guid.NewGuid(),
                UserName = "bob",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234")
            },
            new User
            {
                Id = Guid.NewGuid(),
                UserName = "marks",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234")
            }
        );

        await _db.SaveChangesAsync(ct);
    }
}
