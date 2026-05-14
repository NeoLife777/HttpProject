using HttpProject.Server;

var server = new HttpServer();

Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    server.Stop();
    Console.WriteLine("Сервер остановлен");
};

server.Start();