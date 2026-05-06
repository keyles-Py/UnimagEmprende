namespace EventManager.Application.DTOs.CheckIn;

public sealed class CheckInRequest
{
    public Guid Token { get; set; }
    public Guid EventId { get; set; }
}
