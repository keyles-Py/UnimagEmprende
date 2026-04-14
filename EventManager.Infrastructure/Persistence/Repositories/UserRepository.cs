using EventManager.Application.Interfaces;
using EventManager.Domain.Entities;
using EventManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<Role?> GetRoleByTypeAsync(RoleType roleType, CancellationToken cancellationToken = default) =>
        await _context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name == roleType, cancellationToken);
}
