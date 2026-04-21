using EventManager.Application.DTOs.Event;
using EventManager.Application.Interfaces;
using EventManager.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.API.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IEventService eventService, ILogger<EventsController> logger)
    {
        _eventService = eventService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<EventListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<EventListItemResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var events = await _eventService.GetAllAsync(cancellationToken);
            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al listar eventos.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error interno. Intente más tarde." });
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EventResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var eventResponse = await _eventService.GetByIdAsync(id, cancellationToken);
            if (eventResponse is null)
            {
                return NotFound(new { message = $"El evento con identificador '{id}' no fue encontrado." });
            }

            return Ok(eventResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al consultar el evento {EventId}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error interno. Intente más tarde." });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EventResponse>> Create(
        [FromBody] CreateEventRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _eventService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (InvalidEventCapacityException ex)
        {
            _logger.LogWarning("Capacidad inválida al crear evento: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Error de negocio al crear evento: {Message}", ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al crear evento.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error interno. Intente más tarde." });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EventResponse>> Update(
        Guid id,
        [FromBody] UpdateEventRequest request,
        CancellationToken cancellationToken)
    {
        if (id != request.Id)
        {
            return BadRequest(new { message = "El identificador de la ruta no coincide con el del cuerpo de la solicitud." });
        }

        try
        {
            var response = await _eventService.UpdateAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (EventNotFoundException ex)
        {
            _logger.LogWarning("Evento no encontrado al actualizar: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidEventCapacityException ex)
        {
            _logger.LogWarning("Capacidad inválida al actualizar evento: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Error de negocio al actualizar evento: {Message}", ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar evento {EventId}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error interno. Intente más tarde." });
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _eventService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (EventNotFoundException ex)
        {
            _logger.LogWarning("Evento no encontrado al eliminar: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al eliminar evento {EventId}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error interno. Intente más tarde." });
        }
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EventResponse>> ChangeStatus(
        Guid id,
        [FromBody] ChangeEventStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (id != request.Id)
        {
            return BadRequest(new { message = "El identificador de la ruta no coincide con el del cuerpo de la solicitud." });
        }

        try
        {
            var response = await _eventService.ChangeStatusAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (EventNotFoundException ex)
        {
            _logger.LogWarning("Evento no encontrado al cambiar estado: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Transición de estado inválida: {Message}", ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al cambiar estado del evento {EventId}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error interno. Intente más tarde." });
        }
    }
}
