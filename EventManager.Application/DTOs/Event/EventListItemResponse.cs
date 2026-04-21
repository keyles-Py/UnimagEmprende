using EventManager.Domain.Enums;

namespace EventManager.Application.DTOs.Event;

public sealed class EventListItemResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public string? Location { get; set; }
    public EventStatus Status { get; set; }
    public int MaxCapacity { get; set; }
    public bool HasParking { get; set; }
    public int? ParkingCapacity { get; set; }
}
