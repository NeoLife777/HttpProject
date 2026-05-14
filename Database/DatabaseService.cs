using System;
using System.IO;
using Microsoft.Data.Sqlite;
using HttpProject.Models;

namespace HttpProject.Database
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbPath = Path.Combine(folder, "HttpAuthServer.db");
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            SqliteCommand command = (SqliteCommand)connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS AuthLog (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Timestamp TEXT NOT NULL,
                    Ip TEXT NOT NULL,
                    Login TEXT NOT NULL,
                    Password TEXT NOT NULL,
                    Status TEXT NOT NULL
                )";
            command.ExecuteNonQuery();

            command.Dispose();
            connection.Close();
            // connection.Dispose(); // ← УБРАТЬ!
        }

        public void SaveLog(LogEntry log)
        {
            SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            SqliteCommand command = (SqliteCommand)connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO AuthLog (Timestamp, Ip, Login, Password, Status)
                VALUES (@timestamp, @ip, @login, @password, @status)";

            command.Parameters.AddWithValue("@timestamp", log.Timestamp);
            command.Parameters.AddWithValue("@ip", log.Ip);
            command.Parameters.AddWithValue("@login", log.Login);
            command.Parameters.AddWithValue("@password", log.Password);
            command.Parameters.AddWithValue("@status", log.Status);
            command.ExecuteNonQuery();

            command.Dispose();
            connection.Close();
            // connection.Dispose(); // ← УБРАТЬ!
        }
    }
}
