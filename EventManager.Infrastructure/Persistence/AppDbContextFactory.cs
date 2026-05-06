using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EventManager.Infrastructure.Persistence;

/// <summary>
/// Permite ejecutar "dotnet ef migrations add" sin necesitar DATABASE_URL en el entorno.
/// Solo se usa en tiempo de diseño (herramientas EF CLI).
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=eventmanager_design;Username=postgres;Password=postgres")
            .Options;

        return new AppDbContext(options);
    }
}
