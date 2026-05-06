using EventManager.Application.DTOs.Registration;

namespace EventManager.Application.Interfaces;

public interface IRegistrationService
{
    Task<RegistrationResponse> RegisterAsync(RegisterToEventRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserRegistrationResponse>> GetUserRegistrationsAsync(Guid userId, CancellationToken cancellationToken = default);
}
