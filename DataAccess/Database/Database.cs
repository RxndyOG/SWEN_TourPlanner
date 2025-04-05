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

        /* Difficulty types */

        /// <summary>
        ///     Retrieves the difficulty type with the given id
        /// </summary>
        /// <param name="id">ID of the difficulty type</param>
        /// <returns>The difficulty type</returns>
        public string GetDifficultyType(int id)
        {
            string? difficulty = "";
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT difficulty FROM difficulty_types WHERE id = @id LIMIT 1", connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    difficulty = cmd.ExecuteScalar()?.ToString() ?? "";
                }
                connection.Close();
            }
            return difficulty;
        }

        /* Transportation types */

        /// <summary>
        ///     Retrieves the transportation type with the given id
        /// </summary>
        /// <param name="id">ID of the transportation type</param>
        /// <returns>The transportation type</returns>
        public string GetTransportationType(int id)
        {
            string? transportation_type = "";
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT transportation_type FROM transportation_types WHERE id = @id LIMIT 1", connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    transportation_type = cmd.ExecuteScalar()?.ToString() ?? "";
                }
                connection.Close();
            }
            return transportation_type;
        }

        public string GetConnectionString()
        {
            return connectionString;
        }
    }
}
