using EventManager.Application.DTOs.Email;
using EventManager.Application.Email;
using EventManager.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EventManager.Application.Services;

public sealed class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IQrCodeGenerator _qrCodeGenerator;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IEmailService emailService,
        IRegistrationRepository registrationRepository,
        IEventRepository eventRepository,
        IQrCodeGenerator qrCodeGenerator,
        ILogger<NotificationService> logger)
    {
        _emailService = emailService;
        _registrationRepository = registrationRepository;
        _eventRepository = eventRepository;
        _qrCodeGenerator = qrCodeGenerator;
        _logger = logger;
    }

    public async Task SendRegistrationConfirmationAsync(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var registration = await _registrationRepository.GetWithDetailsAsync(eventId, userId, cancellationToken);
        if (registration is null)
        {
            _logger.LogWarning("Confirmación omitida: inscripción no encontrada EventId={EventId} UserId={UserId}", eventId, userId);
            return;
        }

        var qrBytes = _qrCodeGenerator.Generate($"registration:{eventId}:{userId}");

        var message = new EmailMessage
        {
            To = registration.User.Email,
            Subject = $"Confirmación de inscripción: {registration.Event.Name}",
            HtmlBody = EmailTemplates.RegistrationConfirmation(registration),
            Attachments = new List<EmailAttachment>
            {
                new() { FileName = "codigo_qr.png", Content = qrBytes, ContentType = "image/png" }
            }
        };

        await _emailService.SendAsync(message, cancellationToken);
        _logger.LogInformation("Confirmación enviada a {Email} — EventId={EventId}", registration.User.Email, eventId);
    }

    public async Task SendEventReminderAsync(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var registration = await _registrationRepository.GetWithDetailsAsync(eventId, userId, cancellationToken);
        if (registration is null)
        {
            _logger.LogWarning("Recordatorio omitido: inscripción no encontrada EventId={EventId} UserId={UserId}", eventId, userId);
            return;
        }

        var message = new EmailMessage
        {
            To = registration.User.Email,
            Subject = $"Recordatorio: {registration.Event.Name} es mañana",
            HtmlBody = EmailTemplates.EventReminder(registration)
        };

        await _emailService.SendAsync(message, cancellationToken);
        _logger.LogInformation("Recordatorio enviado a {Email} — EventId={EventId}", registration.User.Email, eventId);
    }

    public async Task SendEventChangedNotificationAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var ev = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (ev is null)
        {
            _logger.LogWarning("Notificación de cambio omitida: evento {EventId} no encontrado", eventId);
            return;
        }

        var registrations = await _registrationRepository.GetRegistrationsByEventAsync(eventId, cancellationToken);

        foreach (var reg in registrations)
        {
            var message = new EmailMessage
            {
                To = reg.User.Email,
                Subject = $"Actualización del evento: {ev.Name}",
                HtmlBody = EmailTemplates.EventChanged(reg.User, ev)
            };
            await _emailService.SendAsync(message, cancellationToken);
        }

        _logger.LogInformation(
            "Notificaciones de cambio enviadas a {Count} inscritos — EventId={EventId}",
            registrations.Count, eventId);
    }

    public async Task SendCheckInConfirmationAsync(
        Guid eventId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var registration = await _registrationRepository.GetWithDetailsAsync(eventId, userId, cancellationToken);
        if (registration is null)
        {
            _logger.LogWarning("Check-in omitido: inscripción no encontrada EventId={EventId} UserId={UserId}", eventId, userId);
            return;
        }

        var message = new EmailMessage
        {
            To = registration.User.Email,
            Subject = $"Asistencia confirmada: {registration.Event.Name}",
            HtmlBody = EmailTemplates.CheckInConfirmation(registration)
        };

        await _emailService.SendAsync(message, cancellationToken);
        _logger.LogInformation("Check-in confirmado enviado a {Email} — EventId={EventId}", registration.User.Email, eventId);
    }
}
