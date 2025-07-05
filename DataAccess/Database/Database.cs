using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DataAccess.Database
{
    public class Database : IDatabase
    {
        private string connectionString { get; }
        private NpgsqlConnection? connection;

        public Database()
        {
            // Get directory of config file
            string baseDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName ?? "";

            // Load configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(baseDirectory)  // Set base directory
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load JSON config
                .Build();

            // Read database settings from config
            string? host = config["Database:Host"];
            string? port = config["Database:Port"];
            string? username = config["Database:Username"];
            string? password = config["Database:Password"];
            string? databaseName = config["Database:DatabaseName"];

            // Construct connection string
            connectionString = $"Host={host};Port={port};Username={username};Password={password};Database={databaseName}";
        }

        /// <summary>
        ///     Connects to the database via config file in base directory
        /// </summary>
        public void ConnectDatabase()
        {
            Console.WriteLine("Connecting to database...");
            using (connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connected successfully!");
                    connection.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        public string GetConnectionString()
        {
            return connectionString;
        }
    }
}
