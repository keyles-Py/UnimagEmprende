using EventManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventManager.Infrastructure.Extensions;

/// <summary>
/// Punto central de registro de dependencias de la capa Infrastructure.
/// Se invoca desde Program.cs en la API.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration["DATABASE_URL"]
            ?? throw new InvalidOperationException(
                "La variable de entorno DATABASE_URL no está configurada.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Los repositorios e interfaces se registrarán aquí a partir del PBI 2.

        return services;
    }
}
