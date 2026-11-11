namespace TaskBoard.Api.Domain;

public class TaskItem
{
    public int Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }

    private TaskItem() { }

    public static TaskItem Create(string title, string? description) => new()
    {
        Title = title.Trim(),
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
        CreatedAtUtc = DateTime.UtcNow
    };

    public void Complete()
    {
        if (IsCompleted) return;
        IsCompleted = true;
        CompletedAtUtc = DateTime.UtcNow;
    }
}
