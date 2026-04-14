using EventManager.Application.DTOs.Auth;
using EventManager.Application.Interfaces;
using EventManager.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    /// <response code="201">Usuario creado exitosamente.</response>
    /// <response code="400">Datos de entrada inválidos (validación FluentValidation).</response>
    /// <response code="409">El email ya está registrado.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegisterResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.RegisterAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Register), new { id = response.Id }, response);
        }
        catch (EmailAlreadyExistsException ex)
        {
            _logger.LogWarning("Conflicto al registrar: {Message}", ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al registrar usuario.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error interno. Intente más tarde." });
        }
    }
}
