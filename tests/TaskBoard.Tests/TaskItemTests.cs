using TaskBoard.Api.Domain;

namespace TaskBoard.Tests;

public class TaskItemTests
{
    [Fact]
    public void Create_RecortaEspaciosYQuedaPendiente()
    {
        var task = TaskItem.Create("  Entrega 1  ", "   ");

        Assert.Equal("Entrega 1", task.Title);
        Assert.Null(task.Description);
        Assert.False(task.IsCompleted);
        Assert.Null(task.CompletedAtUtc);
    }

    [Fact]
    public void Complete_MarcaComoCompletadaConFecha()
    {
        var task = TaskItem.Create("Entrega 1", null);

        task.Complete();

        Assert.True(task.IsCompleted);
        Assert.NotNull(task.CompletedAtUtc);
    }

    [Fact]
    public void Complete_EsIdempotente()
    {
        var task = TaskItem.Create("Entrega 1", null);
        task.Complete();
        var firstCompletion = task.CompletedAtUtc;

        task.Complete();

        Assert.Equal(firstCompletion, task.CompletedAtUtc);
    }
}
