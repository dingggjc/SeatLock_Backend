using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SeatLock.Application.Common.Interfaces;

namespace SeatLock.Persistence.Data;

public sealed class SeatLockDbContextFactory : IDesignTimeDbContextFactory<SeatLockDbContext>
{
    public SeatLockDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SeatLockDbContext>()
            .UseSqlServer(ReadDevelopmentConnectionString())
            .Options;
        return new SeatLockDbContext(options, new DesignTimeTenantContext());
    }

    private static string ReadDevelopmentConnectionString()
    {
        var settingsPath = Path.Combine(FindWebApiContentRoot(), "appsettings.Development.json");
        if (!File.Exists(settingsPath))
        {
            throw new InvalidOperationException("SeatLock.WebApi/appsettings.Development.json is required for local EF Core commands.");
        }

        using var document = JsonDocument.Parse(File.ReadAllText(settingsPath));
        if (!document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings)
            || !connectionStrings.TryGetProperty("DefaultConnection", out var connectionString)
            || string.IsNullOrWhiteSpace(connectionString.GetString()))
        {
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required in appsettings.Development.json.");
        }

        return connectionString.GetString()!;
    }

    private static string FindWebApiContentRoot()
    {
        for (var directory = new DirectoryInfo(Directory.GetCurrentDirectory()); directory is not null; directory = directory.Parent)
        {
            var projectDirectory = Path.Combine(directory.FullName, "SeatLock.WebApi");
            if (File.Exists(Path.Combine(projectDirectory, "appsettings.json")))
            {
                return projectDirectory;
            }

            if (directory.Name == "SeatLock.WebApi" && File.Exists(Path.Combine(directory.FullName, "appsettings.json")))
            {
                return directory.FullName;
            }
        }

        throw new InvalidOperationException("Unable to find SeatLock.WebApi/appsettings.json from the current directory.");
    }

    private sealed class DesignTimeTenantContext : ITenantContext
    {
        public Guid? TenantId => null;
    }
}
