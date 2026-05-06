using EventManager.Application.Interfaces;
using EventManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Infrastructure.Persistence.Repositories;

public sealed class EventRepository : IEventRepository
{
    private readonly AppDbContext _context;

    public EventRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Events
            .AsNoTracking()
            .Include(e => e.Organizer)
            .Include(e => e.EventFiles)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Event>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Events
            .AsNoTracking()
            .Include(e => e.Organizer)
            .OrderBy(e => e.StartDate)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Event>> GetByOrganizerAsync(Guid organizerId, CancellationToken cancellationToken = default) =>
        await _context.Events
            .AsNoTracking()
            .Where(e => e.OrganizerId == organizerId)
            .OrderBy(e => e.StartDate)
            .ToListAsync(cancellationToken);

    public async Task<Event> CreateAsync(Event eventEntity, CancellationToken cancellationToken = default)
    {
        await _context.Events.AddAsync(eventEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return eventEntity;
    }

    public async Task<Event> UpdateAsync(Event eventEntity, CancellationToken cancellationToken = default)
    {
        _context.Events.Update(eventEntity);
        await _context.SaveChangesAsync(cancellationToken);
        return eventEntity;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var eventEntity = await _context.Events.FindAsync(new object[] { id }, cancellationToken);
        if (eventEntity is null)
        {
            return false;
        }

        _context.Events.Remove(eventEntity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExistsByNameAndDateAsync(string name, DateTime startDate, CancellationToken cancellationToken = default) =>
        await _context.Events
            .AsNoTracking()
            .AnyAsync(e => e.Name == name && e.StartDate == startDate, cancellationToken);
}
