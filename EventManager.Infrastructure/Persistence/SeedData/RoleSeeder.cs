using EventManager.Domain.Entities;
using EventManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Infrastructure.Persistence.SeedData;

public static class RoleSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        var rolesToSeed = new[]
        {
            new { Name = RoleType.Admin, Description = "Administrador del sistema con acceso completo." },
            new { Name = RoleType.User,  Description = "Usuario estándar de la plataforma." }
        };

        foreach (var roleData in rolesToSeed)
        {
            var exists = await context.Roles.AnyAsync(r => r.Name == roleData.Name);
            if (!exists)
            {
                context.Roles.Add(new Role
                {
                    Id          = Guid.NewGuid(),
                    Name        = roleData.Name,
                    Description = roleData.Description
                });
            }
        }

        await context.SaveChangesAsync();
    }
}
