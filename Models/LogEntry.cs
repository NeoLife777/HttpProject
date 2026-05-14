namespace HttpProject.Models;

public class LogEntry
{
    public int Id { get; set; }
    public string Timestamp { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}