using Microsoft.EntityFrameworkCore;
using TaskTracker.IdentityService.Domain.Entities;

namespace TaskTracker.IdentityService.Infrastructure.Persistence;

public sealed class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasKey(x => x.Id);

            b.Property(x => x.UserName).IsRequired().HasMaxLength(100);
            b.HasIndex(x => x.UserName).IsUnique();

            b.Property(x => x.PasswordHash).IsRequired().HasMaxLength(200);
        });
    }
}
