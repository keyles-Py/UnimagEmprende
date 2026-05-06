namespace EventManager.Application.DTOs.Registration;

public sealed class RegistrationResponse
{
    public Guid EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
}
