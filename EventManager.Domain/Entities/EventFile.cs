using EventManager.Domain.Common;

namespace EventManager.Domain.Entities;

public class EventFile : BaseEntity
{
    public Guid EventId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }

    public Event Event { get; set; } = null!;
}
