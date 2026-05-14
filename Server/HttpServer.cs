using HttpProject.Database;
using HttpProject.Logger;   
using HttpProject.Models;
using System.Net;
using System.Text;

namespace HttpProject.Server; 

public class HttpServer
{
    private readonly HttpListener _listener;
    private readonly DatabaseService _db;
    private readonly ConsoleLogger _logger;
    private bool _isRunning;

    public HttpServer()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://localhost:8090/");
        _db = new DatabaseService();
        _logger = new ConsoleLogger();
    }

    public void Start()
    {
        _listener.Start();
        _isRunning = true;
        Console.WriteLine("HTTP сервер запущен на http://localhost:8090/");
        Console.WriteLine("Для теста: curl -u admin:password123 http://localhost:8090/private");

        while (_isRunning)
        {
            var context = _listener.GetContext();
            ThreadPool.QueueUserWorkItem(_ => ProcessRequest(context));
        }
    }

    public void Stop()
    {
        _isRunning = false;
        _listener.Stop();
        _listener.Close();
    }

    private void ProcessRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        // Создаём запись для лога
        var log = new LogEntry
        {
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Ip = request.RemoteEndPoint?.Address?.ToString() ?? "unknown",
            Login = "",
            Password = "",
            Status = "Failed"
        };

        // Проверяем Basic Auth
        var authHeader = request.Headers["Authorization"];
        bool isAuthorized = false;

        if (authHeader != null && authHeader.StartsWith("Basic "))
        {
            var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
            var decodedBytes = Convert.FromBase64String(encodedCredentials);
            var decodedCredentials = Encoding.UTF8.GetString(decodedBytes);
            var credentials = decodedCredentials.Split(':');

            if (credentials.Length == 2)
            {
                var login = credentials[0];
                var password = credentials[1];
                log.Login = login;
                log.Password = password;

                // Проверка учётных данных
                if (login == "admin" && password == "password123")
                {
                    isAuthorized = true;
                    log.Status = "Success";
                }
            }
        }

        // Сохраняем в БД и консоль
        _db.SaveLog(log);
        _logger.Log(log);

        // Отправляем ответ
        byte[] buffer;
        if (isAuthorized)
        {
            buffer = Encoding.UTF8.GetBytes("Welcome!");
            response.StatusCode = 200;
        }
        else
        {
            buffer = Encoding.UTF8.GetBytes("Unauthorized");
            response.StatusCode = 401;
            response.Headers.Add("WWW-Authenticate", "Basic realm=\"MyServer\"");
        }

        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }
}