using EventManager.Application.DTOs.Registration;
using EventManager.Application.Interfaces;
using EventManager.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.API.Controllers;

[ApiController]
[Route("api/registrations")]
public sealed class RegistrationsController : ControllerBase
{
    private readonly IRegistrationService _registrationService;
    private readonly ILogger<RegistrationsController> _logger;

    public RegistrationsController(IRegistrationService registrationService, ILogger<RegistrationsController> logger)
    {
        _registrationService = registrationService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(RegistrationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrationResponse>> Register(
        [FromBody] RegisterToEventRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _registrationService.RegisterAsync(request, cancellationToken);
            return CreatedAtAction(
                nameof(GetUserRegistrations),
                new { userId = response.UserId },
                response);
        }
        catch (EventNotFoundException ex)
        {
            _logger.LogWarning("Evento no encontrado al inscribir: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (EventNotOpenForRegistrationException ex)
        {
            _logger.LogWarning("Estado no válido para inscripción: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (DuplicateRegistrationException ex)
        {
            _logger.LogWarning("Inscripción duplicada: {Message}", ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (EventFullException ex)
        {
            _logger.LogWarning("Evento sin cupos: {Message}", ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al inscribir usuario al evento.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error interno. Intente más tarde." });
        }
    }

    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<UserRegistrationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<UserRegistrationResponse>>> GetUserRegistrations(
        Guid userId,
        CancellationToken cancellationToken)
    {
        try
        {
            var registrations = await _registrationService.GetUserRegistrationsAsync(userId, cancellationToken);
            return Ok(registrations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al consultar inscripciones del usuario {UserId}.", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error interno. Intente más tarde." });
        }
    }
}
