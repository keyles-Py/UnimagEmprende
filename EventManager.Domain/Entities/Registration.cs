namespace EventManager.Domain.Entities;

public class Registration
{
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public DateTime RegisteredAt { get; set; }

    public Event Event { get; set; } = null!;
    public User User { get; set; } = null!;
}
