using Microsoft.EntityFrameworkCore;
using TaskTracker.TasksService.Domain.Entities;

namespace TaskTracker.TasksService.Infrastructure.Persistence;

public sealed class TasksDbContext : DbContext
{
    public TasksDbContext(DbContextOptions<TasksDbContext> options) : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var e = modelBuilder.Entity<TaskItem>();

        e.ToTable("tasks");

        e.HasKey(x => x.Id);

        e.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        e.Property(x => x.Description)
            .HasMaxLength(2000);

        e.Property(x => x.Priority)
            .HasConversion<int>();

        e.HasIndex(x => x.OwnerUserId);
    }
}
