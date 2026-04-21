using EventManager.Application.Interfaces;
using EventManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Infrastructure.Persistence.Repositories;

public sealed class RegistrationRepository : IRegistrationRepository
{
    private readonly AppDbContext _context;

    public RegistrationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Registration> RegisterAsync(Registration registration, CancellationToken cancellationToken = default)
    {
        await _context.Registrations.AddAsync(registration, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return registration;
    }

    public async Task<bool> ExistsAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default) =>
        await _context.Registrations
            .AsNoTracking()
            .AnyAsync(r => r.EventId == eventId && r.UserId == userId, cancellationToken);

    public async Task<int> GetCountByEventAsync(Guid eventId, CancellationToken cancellationToken = default) =>
        await _context.Registrations
            .AsNoTracking()
            .CountAsync(r => r.EventId == eventId, cancellationToken);

    public async Task<IReadOnlyList<Registration>> GetRegistrationsByUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _context.Registrations
            .AsNoTracking()
            .Include(r => r.Event)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.RegisteredAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Registration>> GetRegistrationsByEventAsync(Guid eventId, CancellationToken cancellationToken = default) =>
        await _context.Registrations
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.EventId == eventId)
            .OrderByDescending(r => r.RegisteredAt)
            .ToListAsync(cancellationToken);
}
