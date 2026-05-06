using EventManager.Domain.Enums;

namespace EventManager.Application.DTOs.Event;

public sealed class ChangeEventStatusRequest
{
    public Guid Id { get; set; }
    public EventStatus Status { get; set; }
}
