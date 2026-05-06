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

    /// <summary>
    /// Inicia sesión y retorna un JWT Bearer token.
    /// </summary>
    /// <response code="200">Login exitoso, retorna token y datos del usuario.</response>
    /// <response code="400">Datos de entrada inválidos (validación FluentValidation).</response>
    /// <response code="401">Credenciales incorrectas o usuario inactivo.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidCredentialsException)
        {
            _logger.LogWarning("Login fallido para el email {Email}.", request.Email);
            return Unauthorized(new { message = "Las credenciales proporcionadas son inválidas." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al intentar login.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error interno. Intente más tarde." });
        }
    }
}
