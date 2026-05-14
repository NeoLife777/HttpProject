<<<<<<< HEAD
using HttpProject.Models;

namespace HttpProject.Logger;

public class ConsoleLogger
{
    public void Log(LogEntry log)
    {
        Console.WriteLine($"[{log.Timestamp}] IP: {log.Ip} | Login: {log.Login} | Status: {log.Status}");
    }
}
=======
namespace HttpProject;
>>>>>>> fd5723b8be039a1d7bdccbff2e64dbb6b5a5696c
