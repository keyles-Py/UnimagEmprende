using EventManager.Application.Interfaces;
using EventManager.Application.Services;
using EventManager.Infrastructure.Extensions;
using EventManager.Infrastructure.Persistence;
using EventManager.Infrastructure.Persistence.SeedData;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Servicios ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Infrastructure (DbContext, repositorios, PasswordHasher)
builder.Services.AddInfrastructure(builder.Configuration);

// Application services
builder.Services.AddScoped<IAuthService, AuthService>();

// FluentValidation — auto-valida los modelos de entrada antes de llegar al controller
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ── Pipeline ───────────────────────────────────────────────────────────────
var app = builder.Build();

// Aplicar migraciones y seed al inicio
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await RoleSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow
}));

app.Run();
