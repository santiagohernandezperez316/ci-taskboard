using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Contracts;
using TaskBoard.Api.Data;
using TaskBoard.Api.Domain;

var builder = WebApplication.CreateBuilder(args);

// En Docker llega por la variable de entorno ConnectionStrings__Default (ver docker-compose.yml).
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'ConnectionStrings:Default'.");

builder.Services.AddDbContext<TaskBoardDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

await DatabaseInitializer.InitializeAsync(app.Services, app.Logger);

app.MapGet("/", () => Results.Ok(new
{
    service = "TaskBoard API",
    container = Environment.MachineName
}));

// Verifica la comunicación real con el contenedor de PostgreSQL.
app.MapGet("/health", async (TaskBoardDbContext db) =>
    await db.Database.CanConnectAsync()
        ? Results.Ok(new { status = "healthy", database = "reachable" })
        : Results.Problem("La base de datos no responde.", statusCode: StatusCodes.Status503ServiceUnavailable));

var tasks = app.MapGroup("/api/tasks");

tasks.MapGet("/", async (TaskBoardDbContext db) =>
    await db.Tasks.AsNoTracking().OrderBy(t => t.Id).ToListAsync());

tasks.MapGet("/{id:int}", async (int id, TaskBoardDbContext db) =>
    await db.Tasks.FindAsync(id) is { } task ? Results.Ok(task) : Results.NotFound());

tasks.MapPost("/", async (CreateTaskRequest request, TaskBoardDbContext db) =>
{
    var errors = TaskValidator.Validate(request.Title);
    if (errors.Count > 0)
        return Results.ValidationProblem(new Dictionary<string, string[]> { ["title"] = errors.ToArray() });

    var task = TaskItem.Create(request.Title!, request.Description);
    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tasks/{task.Id}", task);
});

tasks.MapPut("/{id:int}/complete", async (int id, TaskBoardDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null) return Results.NotFound();

    task.Complete();
    await db.SaveChangesAsync();
    return Results.Ok(task);
});

tasks.MapDelete("/{id:int}", async (int id, TaskBoardDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null) return Results.NotFound();

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

public partial class Program { }
