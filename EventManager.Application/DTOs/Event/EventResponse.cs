using EventManager.Domain.Enums;

namespace EventManager.Application.DTOs.Event;

public sealed class EventResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int MaxCapacity { get; set; }
    public bool HasParking { get; set; }
    public int? ParkingCapacity { get; set; }
    public EventStatus Status { get; set; }
    public Guid OrganizerId { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
