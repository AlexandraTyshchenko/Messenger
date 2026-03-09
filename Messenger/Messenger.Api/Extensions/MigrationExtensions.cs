using Messenger.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("Migration");

        try
        {
            var dbContext = scope.ServiceProvider
                .GetRequiredService<ApplicationContext>();

            var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();

            if (pendingMigrations.Any())
            {
                logger.LogInformation(
                    "Applying {Count} pending migrations: {Migrations}",
                    pendingMigrations.Count,
                    string.Join(", ", pendingMigrations));

                dbContext.Database.Migrate();

                logger.LogInformation("Migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("No pending migrations.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Migration failed.");
        }
    }
}