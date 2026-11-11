using Microsoft.EntityFrameworkCore;

namespace TaskBoard.Api.Data;

/// <summary>
/// Crea el esquema al arrancar. Reintenta porque, aunque compose espera el healthcheck
/// de PostgreSQL, la base puede tardar unos segundos más en aceptar conexiones.
/// </summary>
public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, ILogger logger, int maxAttempts = 10)
    {
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                using var scope = services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TaskBoardDbContext>();
                await db.Database.EnsureCreatedAsync();
                logger.LogInformation("Base de datos lista (intento {Attempt}).", attempt);
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                logger.LogWarning("No se pudo conectar a la base de datos (intento {Attempt}/{Max}): {Message}",
                    attempt, maxAttempts, ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        }
    }
}
