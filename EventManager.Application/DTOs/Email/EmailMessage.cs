namespace EventManager.Application.DTOs.Email;

public sealed class EmailMessage
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string HtmlBody { get; set; } = string.Empty;
    public List<EmailAttachment> Attachments { get; set; } = new();
}
