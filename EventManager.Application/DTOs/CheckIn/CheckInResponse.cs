namespace EventManager.Application.DTOs.CheckIn;

public sealed class CheckInResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? UserFullName { get; set; }
    public string? EventName { get; set; }
    public DateTime? CheckedInAt { get; set; }
}
