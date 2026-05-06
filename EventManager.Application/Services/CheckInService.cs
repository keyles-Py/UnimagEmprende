using System.Globalization;
using EventManager.Application.DTOs.CheckIn;
using EventManager.Application.Interfaces;
using EventManager.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace EventManager.Application.Services;

public sealed class CheckInService : ICheckInService
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IEmailJobService _emailJobService;
    private readonly ILogger<CheckInService> _logger;

    private static readonly CultureInfo Es = new("es-CO");

    public CheckInService(
        IRegistrationRepository registrationRepository,
        IEventRepository eventRepository,
        IEmailJobService emailJobService,
        ILogger<CheckInService> logger)
    {
        _registrationRepository = registrationRepository;
        _eventRepository = eventRepository;
        _emailJobService = emailJobService;
        _logger = logger;
    }

    public async Task<CheckInResponse> ValidateAndCheckInAsync(
        CheckInRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validando QR Token={Token} EventId={EventId}", request.Token, request.EventId);

        var registration = await _registrationRepository.GetByTokenAsync(request.Token, cancellationToken);

        if (registration is null)
        {
            _logger.LogWarning("Token no encontrado: {Token}", request.Token);
            return new CheckInResponse
            {
                Success = false,
                Message = "QR inválido: el código no corresponde a ninguna inscripción."
            };
        }

        if (registration.EventId != request.EventId)
        {
            _logger.LogWarning(
                "Token {Token} pertenece al evento {RegistrationEventId} pero se validó en {RequestEventId}",
                request.Token, registration.EventId, request.EventId);

            return new CheckInResponse
            {
                Success = false,
                Message = "El código QR no corresponde a este evento."
            };
        }

        if (registration.CheckedIn)
        {
            var usedAt = registration.CheckedInAt?.ToString("d 'de' MMMM 'de' yyyy 'a las' HH:mm 'UTC'", Es)
                         ?? "fecha desconocida";

            _logger.LogWarning("Token ya utilizado: {Token} — CheckIn el {UsedAt}", request.Token, usedAt);

            return new CheckInResponse
            {
                Success = false,
                Message = $"Este código QR ya fue utilizado. Asistencia registrada el {usedAt}.",
                UserFullName = $"{registration.User.FirstName} {registration.User.LastName}",
                EventName = registration.Event.Name,
                CheckedInAt = registration.CheckedInAt
            };
        }

        var now = DateTime.UtcNow;
        await _registrationRepository.UpdateCheckInAsync(
            registration.EventId, registration.UserId, now, cancellationToken);

        _emailJobService.EnqueueCheckInConfirmation(registration.EventId, registration.UserId);

        _logger.LogInformation(
            "Check-in exitoso — UserId={UserId} EventId={EventId} Token={Token}",
            registration.UserId, registration.EventId, request.Token);

        return new CheckInResponse
        {
            Success = true,
            Message = "¡Check-in exitoso! Asistencia registrada correctamente.",
            UserFullName = $"{registration.User.FirstName} {registration.User.LastName}",
            EventName = registration.Event.Name,
            CheckedInAt = now
        };
    }

    public async Task<AttendanceReportResponse> GetAttendanceReportAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var ev = await _eventRepository.GetByIdAsync(eventId, cancellationToken)
            ?? throw new EventNotFoundException(eventId);

        var registrations = await _registrationRepository.GetRegistrationsByEventAsync(eventId, cancellationToken);

        var totalRegistered = registrations.Count;
        var totalCheckedIn = registrations.Count(r => r.CheckedIn);
        var percentage = totalRegistered > 0
            ? Math.Round((double)totalCheckedIn / totalRegistered * 100, 1)
            : 0.0;

        return new AttendanceReportResponse
        {
            EventId = eventId,
            EventName = ev.Name,
            TotalRegistered = totalRegistered,
            TotalCheckedIn = totalCheckedIn,
            AttendancePercentage = percentage,
            Attendees = registrations
                .Select(r => new AttendeeInfo
                {
                    FullName = $"{r.User.FirstName} {r.User.LastName}",
                    Email = r.User.Email,
                    CheckedIn = r.CheckedIn,
                    CheckedInAt = r.CheckedInAt
                })
                .OrderBy(a => a.FullName)
                .ToList()
        };
    }
}
