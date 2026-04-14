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
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
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
}
