using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Domain;

namespace TaskBoard.Api.Data;

public class TaskBoardDbContext(DbContextOptions<TaskBoardDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var task = modelBuilder.Entity<TaskItem>();
        task.ToTable("tasks");
        task.HasKey(t => t.Id);
        task.Property(t => t.Title).IsRequired().HasMaxLength(TaskValidator.MaxTitleLength);
        task.Property(t => t.Description).HasMaxLength(500);
    }
}
