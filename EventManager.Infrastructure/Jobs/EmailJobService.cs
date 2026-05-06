using EventManager.Application.Interfaces;
using Hangfire;

namespace EventManager.Infrastructure.Jobs;

public sealed class EmailJobService : IEmailJobService
{
    private readonly IBackgroundJobClient _jobs;

    public EmailJobService(IBackgroundJobClient jobs)
    {
        _jobs = jobs;
    }

    public void EnqueueRegistrationConfirmation(Guid eventId, Guid userId)
        => _jobs.Enqueue<INotificationService>(s =>
            s.SendRegistrationConfirmationAsync(eventId, userId, CancellationToken.None));

    public void ScheduleEventReminder(Guid eventId, Guid userId, DateTime eventStartDate)
    {
        var fireAt = eventStartDate.AddHours(-24);
        var delay = fireAt - DateTime.UtcNow;

        if (delay > TimeSpan.Zero)
        {
            _jobs.Schedule<INotificationService>(s =>
                s.SendEventReminderAsync(eventId, userId, CancellationToken.None), delay);
        }
        else
        {
            // El evento es en menos de 24 h: enviar recordatorio inmediatamente
            _jobs.Enqueue<INotificationService>(s =>
                s.SendEventReminderAsync(eventId, userId, CancellationToken.None));
        }
    }

    public void EnqueueEventChangedNotification(Guid eventId)
        => _jobs.Enqueue<INotificationService>(s =>
            s.SendEventChangedNotificationAsync(eventId, CancellationToken.None));

    public void EnqueueEventReminder(Guid eventId, Guid userId)
        => _jobs.Enqueue<INotificationService>(s =>
            s.SendEventReminderAsync(eventId, userId, CancellationToken.None));

    public void EnqueueCheckInConfirmation(Guid eventId, Guid userId)
        => _jobs.Enqueue<INotificationService>(s =>
            s.SendCheckInConfirmationAsync(eventId, userId, CancellationToken.None));
}
