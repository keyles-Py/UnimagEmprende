using EventManager.Domain.Entities;

namespace EventManager.Application.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Event>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Event>> GetByOrganizerAsync(Guid organizerId, CancellationToken cancellationToken = default);
    Task<Event> CreateAsync(Event eventEntity, CancellationToken cancellationToken = default);
    Task<Event> UpdateAsync(Event eventEntity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAndDateAsync(string name, DateTime startDate, CancellationToken cancellationToken = default);
}
