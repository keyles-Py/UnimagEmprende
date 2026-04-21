namespace EventManager.Application.DTOs.Registration;

public sealed class RegisterToEventRequest
{
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
}
