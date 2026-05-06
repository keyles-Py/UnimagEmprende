using EventManager.Application.DTOs.Event;
using EventManager.Application.Interfaces;
using EventManager.Domain.Entities;
using EventManager.Domain.Enums;
using EventManager.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace EventManager.Application.Services;

public sealed class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IEmailJobService _emailJobService;
    private readonly ILogger<EventService> _logger;

    public EventService(
        IEventRepository eventRepository,
        IEmailJobService emailJobService,
        ILogger<EventService> logger)
    {
        _eventRepository = eventRepository;
        _emailJobService = emailJobService;
        _logger = logger;
    }

    public async Task<EventResponse> CreateAsync(CreateEventRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creando evento '{EventName}'", request.Name);

        if (request.MaxCapacity <= 0)
        {
            throw new InvalidEventCapacityException(request.MaxCapacity);
        }

        if (request.EndDate.HasValue && request.EndDate <= request.StartDate)
        {
            throw new InvalidOperationException("La fecha de fin debe ser posterior a la fecha de inicio.");
        }

        var exists = await _eventRepository.ExistsByNameAndDateAsync(request.Name, request.StartDate, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException($"Ya existe un evento con el nombre '{request.Name}' en la misma fecha de inicio.");
        }

        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Location = request.Location,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            MaxCapacity = request.MaxCapacity,
            HasParking = request.HasParking,
            Status = EventStatus.Borrador,
            OrganizerId = request.OrganizerId
        };

        var created = await _eventRepository.CreateAsync(eventEntity, cancellationToken);

        _logger.LogInformation("Evento creado exitosamente. Id: {EventId}", created.Id);

        return MapToResponse(created);
    }

    public async Task<EventResponse> UpdateAsync(UpdateEventRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Actualizando evento '{EventId}'", request.Id);

        var eventEntity = await _eventRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new EventNotFoundException(request.Id);

        if (request.MaxCapacity <= 0)
        {
            throw new InvalidEventCapacityException(request.MaxCapacity);
        }

        if (request.EndDate.HasValue && request.EndDate <= request.StartDate)
        {
            throw new InvalidOperationException("La fecha de fin debe ser posterior a la fecha de inicio.");
        }

        var hasKeyChange =
            eventEntity.StartDate != request.StartDate ||
            eventEntity.EndDate != request.EndDate ||
            eventEntity.Location != request.Location;

        eventEntity.Name = request.Name;
        eventEntity.Description = request.Description;
        eventEntity.Location = request.Location;
        eventEntity.StartDate = request.StartDate;
        eventEntity.EndDate = request.EndDate;
        eventEntity.MaxCapacity = request.MaxCapacity;
        eventEntity.HasParking = request.HasParking;
        eventEntity.Status = request.Status;

        var updated = await _eventRepository.UpdateAsync(eventEntity, cancellationToken);

        if (hasKeyChange)
        {
            _emailJobService.EnqueueEventChangedNotification(updated.Id);
            _logger.LogInformation("Notificación de cambio encolada para EventId={EventId}", updated.Id);
        }

        _logger.LogInformation("Evento actualizado exitosamente. Id: {EventId}", updated.Id);

        return MapToResponse(updated);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Eliminando evento '{EventId}'", id);

        var deleted = await _eventRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            throw new EventNotFoundException(id);
        }

        _logger.LogInformation("Evento eliminado exitosamente. Id: {EventId}", id);
        return true;
    }

    public async Task<EventResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(id, cancellationToken);
        return eventEntity is null ? null : MapToResponse(eventEntity);
    }

    public async Task<IReadOnlyList<EventListItemResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var events = await _eventRepository.GetAllAsync(cancellationToken);
        return events.Select(MapToListItemResponse).ToList();
    }

    public async Task<EventResponse> ChangeStatusAsync(ChangeEventStatusRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cambiando estado del evento '{EventId}' a '{NewStatus}'", request.Id, request.Status);

        var eventEntity = await _eventRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new EventNotFoundException(request.Id);

        ValidateStatusTransition(eventEntity.Status, request.Status);

        eventEntity.Status = request.Status;
        var updated = await _eventRepository.UpdateAsync(eventEntity, cancellationToken);

        _logger.LogInformation("Estado del evento '{EventId}' actualizado a '{NewStatus}'", updated.Id, updated.Status);

        return MapToResponse(updated);
    }

    private static void ValidateStatusTransition(EventStatus current, EventStatus next)
    {
        if (current == EventStatus.Finalizado && next != EventStatus.Finalizado)
        {
            throw new InvalidOperationException("No se puede cambiar el estado de un evento finalizado.");
        }

        if (current == EventStatus.Cancelado && next != EventStatus.Cancelado)
        {
            throw new InvalidOperationException("No se puede cambiar el estado de un evento cancelado.");
        }
    }

    private static EventResponse MapToResponse(Event eventEntity)
    {
        return new EventResponse
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            Location = eventEntity.Location,
            StartDate = eventEntity.StartDate,
            EndDate = eventEntity.EndDate,
            MaxCapacity = eventEntity.MaxCapacity,
            HasParking = eventEntity.HasParking,
            Status = eventEntity.Status,
            OrganizerId = eventEntity.OrganizerId,
            OrganizerName = $"{eventEntity.Organizer.FirstName} {eventEntity.Organizer.LastName}",
            CreatedAt = eventEntity.CreatedAt,
            UpdatedAt = eventEntity.UpdatedAt
        };
    }

    private static EventListItemResponse MapToListItemResponse(Event eventEntity)
    {
        return new EventListItemResponse
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            StartDate = eventEntity.StartDate,
            Location = eventEntity.Location,
            Status = eventEntity.Status,
            MaxCapacity = eventEntity.MaxCapacity,
            HasParking = eventEntity.HasParking
        };
    }
}
