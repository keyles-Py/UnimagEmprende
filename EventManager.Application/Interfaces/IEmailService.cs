using EventManager.Application.DTOs.Email;

namespace EventManager.Application.Interfaces;

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
