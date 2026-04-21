namespace EventManager.Application.DTOs.Event;

public sealed class CreateEventRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int MaxCapacity { get; set; }
    public bool HasParking { get; set; }
    public Guid OrganizerId { get; set; }
}
