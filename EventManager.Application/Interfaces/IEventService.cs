using EventManager.Application.DTOs.Event;

namespace EventManager.Application.Interfaces;

public interface IEventService
{
    Task<EventResponse> CreateAsync(CreateEventRequest request, CancellationToken cancellationToken = default);
    Task<EventResponse> UpdateAsync(UpdateEventRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EventResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventListItemResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EventResponse> ChangeStatusAsync(ChangeEventStatusRequest request, CancellationToken cancellationToken = default);
}
