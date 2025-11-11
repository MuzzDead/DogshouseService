using DogshouseService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace DogshouseService.DAL.Configurations;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services, ILogger logger, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await db.Database.MigrateAsync(ct);

        if (await db.Dogs.AnyAsync(ct))
        {
            logger.LogInformation("Seed skipped: data already exists.");
            return;
        }

        var dogs = new[]
            {
                new Dog { Name = "Neo", Color = "red & amber", TailLength = 22, Weight = 32},
                new Dog { Name = "Jessy", Color = "black & white", TailLength = 7, Weight = 14 }
            };

        await db.Dogs.AddRangeAsync(dogs, ct);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Seed completed successfully.");
    }
}
