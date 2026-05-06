using EventManager.Application.DTOs.Registration;
using EventManager.Application.Interfaces;
using EventManager.Domain.Entities;
using EventManager.Domain.Enums;
using EventManager.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace EventManager.Application.Services;

public sealed class RegistrationService : IRegistrationService
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IEmailJobService _emailJobService;
    private readonly ILogger<RegistrationService> _logger;

    public RegistrationService(
        IRegistrationRepository registrationRepository,
        IEventRepository eventRepository,
        IEmailJobService emailJobService,
        ILogger<RegistrationService> logger)
    {
        _registrationRepository = registrationRepository;
        _eventRepository = eventRepository;
        _emailJobService = emailJobService;
        _logger = logger;
    }

    public async Task<RegistrationResponse> RegisterAsync(RegisterToEventRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Intentando inscribir usuario '{UserId}' al evento '{EventId}'",
            request.UserId, request.EventId);

        var eventEntity = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken)
            ?? throw new EventNotFoundException(request.EventId);

        // PBI-21: Validación de estado operativo
        if (eventEntity.Status != EventStatus.Publicado)
        {
            _logger.LogWarning(
                "Inscripción rechazada: el evento '{EventId}' no está publicado (estado: {Status}).",
                request.EventId, eventEntity.Status);
            throw new EventNotOpenForRegistrationException(request.EventId, eventEntity.Status);
        }

        // PBI-20: Evitar inscripciones duplicadas
        var alreadyRegistered = await _registrationRepository.ExistsAsync(request.EventId, request.UserId, cancellationToken);
        if (alreadyRegistered)
        {
            _logger.LogWarning(
                "Inscripción rechazada: el usuario '{UserId}' ya está inscrito en el evento '{EventId}'.",
                request.UserId, request.EventId);
            throw new DuplicateRegistrationException(request.EventId, request.UserId);
        }

        // PBI-19: Control de capacidad máxima
        var currentRegistrations = await _registrationRepository.GetCountByEventAsync(request.EventId, cancellationToken);
        if (currentRegistrations >= eventEntity.MaxCapacity)
        {
            _logger.LogWarning(
                "Inscripción rechazada: el evento '{EventId}' ha alcanzado su capacidad máxima ({MaxCapacity}).",
                request.EventId, eventEntity.MaxCapacity);
            throw new EventFullException(request.EventId);
        }

        var registration = new Registration
        {
            EventId = request.EventId,
            UserId = request.UserId,
            RegisteredAt = DateTime.UtcNow
        };

        var created = await _registrationRepository.RegisterAsync(registration, cancellationToken);

        _logger.LogInformation(
            "Usuario '{UserId}' inscrito exitosamente al evento '{EventId}'.",
            request.UserId, request.EventId);

        _emailJobService.EnqueueRegistrationConfirmation(created.EventId, created.UserId);
        _emailJobService.ScheduleEventReminder(created.EventId, created.UserId, eventEntity.StartDate);

        return new RegistrationResponse
        {
            EventId = eventEntity.Id,
            EventName = eventEntity.Name,
            UserId = request.UserId,
            UserName = "", // Se completa en el controller si es necesario, o se deja vacío por ahora
            RegisteredAt = created.RegisteredAt
        };
    }

    public async Task<IReadOnlyList<UserRegistrationResponse>> GetUserRegistrationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Consultando inscripciones del usuario '{UserId}'", userId);

        var registrations = await _registrationRepository.GetRegistrationsByUserAsync(userId, cancellationToken);

        return registrations.Select(r => new UserRegistrationResponse
        {
            EventId = r.Event.Id,
            EventName = r.Event.Name,
            StartDate = r.Event.StartDate,
            Location = r.Event.Location,
            Status = r.Event.Status,
            RegisteredAt = r.RegisteredAt
        }).ToList();
    }
}
