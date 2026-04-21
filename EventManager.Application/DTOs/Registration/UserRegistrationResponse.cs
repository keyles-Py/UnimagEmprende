using EventManager.Domain.Enums;

namespace EventManager.Application.DTOs.Registration;

public sealed class UserRegistrationResponse
{
    public Guid EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public string? Location { get; set; }
    public EventStatus Status { get; set; }
    public DateTime RegisteredAt { get; set; }
}
