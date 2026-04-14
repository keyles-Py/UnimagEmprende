using Microsoft.EntityFrameworkCore;

namespace EventManager.Infrastructure.Persistence;

/// <summary>
/// Contexto principal de base de datos para EventManager.
/// Las entidades de dominio se registrarán aquí a partir del PBI 2.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Las configuraciones de entidades se aplicarán aquí en el PBI 2.
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
