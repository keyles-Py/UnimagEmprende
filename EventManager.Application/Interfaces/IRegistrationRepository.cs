using EventManager.Domain.Entities;

namespace EventManager.Application.Interfaces;

public interface IRegistrationRepository
{
    Task<Registration> RegisterAsync(Registration registration, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetCountByEventAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Registration>> GetRegistrationsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Registration>> GetRegistrationsByEventAsync(Guid eventId, CancellationToken cancellationToken = default);
}
