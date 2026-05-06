using EventManager.Application.Interfaces;
using EventManager.Application.Services;
using EventManager.Infrastructure.Email;
using EventManager.Infrastructure.Jobs;
using EventManager.Infrastructure.Persistence;
using EventManager.Infrastructure.Persistence.Repositories;
using EventManager.Infrastructure.Security;
using Hangfire;
using Hangfire.PostgreSql;
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

        // ── Base de datos ──────────────────────────────────────────────────
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // ── Repositorios ───────────────────────────────────────────────────
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IRegistrationRepository, RegistrationRepository>();

        // ── Seguridad ──────────────────────────────────────────────────────
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // ── Email / Notificaciones ─────────────────────────────────────────
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddSingleton<IQrCodeGenerator, QrCodeGenerator>();
        services.AddScoped<INotificationService, NotificationService>();

        // ── Hangfire (cola de trabajos en background) ──────────────────────
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString)));

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 4;
            options.Queues = new[] { "default" };
        });

        services.AddScoped<IEmailJobService, EmailJobService>();

        return services;
    }
}
