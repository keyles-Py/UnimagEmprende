using EventManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize(Roles = "Admin")]
public sealed class NotificationController : ControllerBase
{
    private readonly IEmailJobService _emailJobService;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(
        IEmailJobService emailJobService,
        IRegistrationRepository registrationRepository,
        ILogger<NotificationController> logger)
    {
        _emailJobService = emailJobService;
        _registrationRepository = registrationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Dispara manualmente los recordatorios a todos los inscritos de un evento.
    /// Útil para pruebas o cuando el job programado no pudo ejecutarse.
    /// </summary>
    /// <response code="202">Recordatorios encolados correctamente.</response>
    /// <response code="401">No autenticado.</response>
    /// <response code="403">Solo administradores.</response>
    [HttpPost("reminders/event/{eventId:guid}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> TriggerReminders(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var registrations = await _registrationRepository.GetRegistrationsByEventAsync(eventId, cancellationToken);

        foreach (var reg in registrations)
        {
            _emailJobService.EnqueueEventReminder(reg.EventId, reg.UserId);
        }

        _logger.LogInformation(
            "Recordatorios manuales encolados para {Count} inscritos — EventId={EventId}",
            registrations.Count, eventId);

        return Accepted(new { message = $"Recordatorios encolados para {registrations.Count} inscrito(s)." });
    }

    /// <summary>
    /// Dispara manualmente la confirmación de check-in para un usuario en un evento.
    /// Útil para pruebas del flujo de asistencia.
    /// </summary>
    /// <response code="202">Confirmación de check-in encolada.</response>
    /// <response code="401">No autenticado.</response>
    /// <response code="403">Solo administradores.</response>
    [HttpPost("checkin/event/{eventId:guid}/user/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult TriggerCheckIn(Guid eventId, Guid userId)
    {
        _emailJobService.EnqueueCheckInConfirmation(eventId, userId);

        _logger.LogInformation(
            "Check-in manual encolado — EventId={EventId} UserId={UserId}", eventId, userId);

        return Accepted(new { message = "Confirmación de check-in encolada." });
    }
}
