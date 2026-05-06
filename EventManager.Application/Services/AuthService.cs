using EventManager.Application.DTOs.Auth;
using EventManager.Application.Interfaces;
using EventManager.Domain.Entities;
using EventManager.Domain.Enums;
using EventManager.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace EventManager.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<RegisterResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Intentando registrar usuario con email {Email}", request.Email);

        var existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
        {
            _logger.LogWarning("Registro rechazado: el email {Email} ya está registrado.", request.Email);
            throw new EmailAlreadyExistsException(request.Email);
        }

        var role = await _userRepository.GetRoleByTypeAsync(RoleType.User, cancellationToken)
            ?? throw new InvalidOperationException("El rol 'User' no existe en la base de datos.");

        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new() { UserId = userId, RoleId = role.Id }
            }
        };

        var created = await _userRepository.CreateAsync(user, cancellationToken);

        _logger.LogInformation("Usuario registrado exitosamente. Id: {UserId}", created.Id);

        return new RegisterResponse
        {
            Id = created.Id,
            FirstName = created.FirstName,
            LastName = created.LastName,
            Email = created.Email
        };
    }

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Intento de login para el email {Email}", request.Email);

        var user = await _userRepository.GetByEmailWithRolesAsync(request.Email, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Login fallido: email {Email} no encontrado.", request.Email);
            throw new InvalidCredentialsException();
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login fallido: contraseña incorrecta para email {Email}.", request.Email);
            throw new InvalidCredentialsException();
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Login fallido: usuario {Email} está inactivo.", request.Email);
            throw new InvalidCredentialsException();
        }

        var roleName = user.UserRoles.FirstOrDefault()?.Role.Name.ToString()
            ?? RoleType.User.ToString();

        var token = _jwtTokenService.GenerateToken(user, roleName);

        _logger.LogInformation("Login exitoso para el usuario {UserId}.", user.Id);

        return new LoginResponse
        {
            Token = token,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            Role = roleName,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }
}
