using EventManager.Application.DTOs.CheckIn;
using EventManager.Application.Interfaces;
using EventManager.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.API.Controllers;

[ApiController]
[Route("api/checkin")]
[Authorize]
public sealed class CheckInController : ControllerBase
{
    private readonly ICheckInService _checkInService;
    private readonly ILogger<CheckInController> _logger;

    public CheckInController(ICheckInService checkInService, ILogger<CheckInController> logger)
    {
        _checkInService = checkInService;
        _logger = logger;
    }

    /// <summary>
    /// Valida un código QR y registra la asistencia del usuario al evento.
    /// Rechaza QRs ya usados, inválidos o de eventos distintos.
    /// </summary>
    /// <response code="200">Resultado de la validación (success: true/false con mensaje).</response>
    /// <response code="400">Datos de entrada inválidos.</response>
    /// <response code="401">No autenticado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(CheckInResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CheckInResponse>> Validate(
        [FromBody] CheckInRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Token == Guid.Empty)
            return BadRequest(new { message = "El token no puede estar vacío." });

        if (request.EventId == Guid.Empty)
            return BadRequest(new { message = "El identificador del evento no puede estar vacío." });

        try
        {
            var result = await _checkInService.ValidateAndCheckInAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar el QR Token={Token}", request.Token);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error interno al procesar el check-in." });
        }
    }

    /// <summary>
    /// Reporte de asistencia vs inscritos para un evento.
    /// Solo accesible por administradores.
    /// </summary>
    /// <response code="200">Reporte con totales, porcentaje y lista de asistentes.</response>
    /// <response code="401">No autenticado.</response>
    /// <response code="403">Requiere rol Admin.</response>
    /// <response code="404">Evento no encontrado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet("report/{eventId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AttendanceReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AttendanceReportResponse>> Report(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        try
        {
            var report = await _checkInService.GetAttendanceReportAsync(eventId, cancellationToken);
            return Ok(report);
        }
        catch (EventNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar reporte de asistencia EventId={EventId}", eventId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error interno al generar el reporte." });
        }
    }
}
