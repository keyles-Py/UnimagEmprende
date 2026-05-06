using EventManager.Domain.Common;

namespace EventManager.Domain.Entities;

public class Event : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int MaxCapacity { get; set; }
    public bool HasParking { get; set; }
    public int? ParkingCapacity { get; set; }
    public Enums.EventStatus Status { get; set; }
    public Guid OrganizerId { get; set; }
    public User Organizer { get; set; } = null!;

    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public ICollection<EventFile> EventFiles { get; set; } = new List<EventFile>();

    public string? ExternalProvider { get; set; }
    public string? ExternalId { get; set; }
}
