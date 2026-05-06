using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EventManager.Infrastructure.Persistence;

/// <summary>
/// Permite ejecutar "dotnet ef migrations add" sin hardcodear credenciales.
/// Cumple con los requisitos de seguridad para el pipeline de CI.
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // 1. Construir la configuración buscando en el directorio actual
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            // Opcional: permite tener un archivo local para desarrollo
            .AddJsonFile("appsettings.json", optional: true)
            // Prioridad a las variables de entorno (Requisito Técnico 4)
            .AddEnvironmentVariables() 
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // 2. Obtener la cadena de conexión de forma dinámica
        // Se busca 'DATABASE_URL' que es el estándar para despliegues
        var connectionString = configuration["DATABASE_URL"] 
                               ?? configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Error: No se encontró 'DATABASE_URL' en las variables de entorno.");
        }

        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}