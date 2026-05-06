namespace EventManager.Application.Interfaces;

public interface IEmailJobService
{
    void EnqueueRegistrationConfirmation(Guid eventId, Guid userId);
    void ScheduleEventReminder(Guid eventId, Guid userId, DateTime eventStartDate);
    void EnqueueEventChangedNotification(Guid eventId);
    void EnqueueEventReminder(Guid eventId, Guid userId);
    void EnqueueCheckInConfirmation(Guid eventId, Guid userId);
}
