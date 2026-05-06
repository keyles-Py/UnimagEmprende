namespace EventManager.Application.Interfaces;

public interface INotificationService
{
    Task SendRegistrationConfirmationAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);
    Task SendEventReminderAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);
    Task SendEventChangedNotificationAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task SendCheckInConfirmationAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);
}
