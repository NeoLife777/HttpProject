using HttpProject.Models;

namespace HttpProject.Logger;

public class ConsoleLogger
{
    public void Log(LogEntry log)
    {
        Console.WriteLine($"[{log.Timestamp}] IP: {log.Ip} | Login: {log.Login} | Status: {log.Status}");
    }
}