namespace EventManager.Application.DTOs.CheckIn;

public sealed class AttendanceReportResponse
{
    public Guid EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public int TotalRegistered { get; set; }
    public int TotalCheckedIn { get; set; }
    public double AttendancePercentage { get; set; }
    public List<AttendeeInfo> Attendees { get; set; } = new();
}

public sealed class AttendeeInfo
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool CheckedIn { get; set; }
    public DateTime? CheckedInAt { get; set; }
}
