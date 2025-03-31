using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.IO;
using Model;

namespace DataAccess
{
    public class Database
    {
        string connectionString = "";
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

        /* Tours */

        /// <summary>
        ///     Creates a tour with the given object
        /// </summary>
        /// <param name="tour">Object of the tour to be created</param>
        public void CreateTour(Tour tour)
        {
            // Check required attributes
            try
            {
                CheckTourRequiredAttributes(tour);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            // Create tour
            using (connection = new NpgsqlConnection(connectionString))
            {
                string query = """
                    INSERT INTO tours (name, description, from_location, to_location, transportation_type, distance, estimated_time, route_information)
                    VALUES (@Name, @Description, @From_location, @To_location, @Transportation_type, @Distance, @Estimated_time, @Route_information)
                    """;
                connection.Query<Tour>(query, tour);
            }
        }

        /// <summary>
        ///     Retrieves the tour with the given id
        /// </summary>
        /// <param name="id">ID of the tour</param>
        /// <returns>Tour object with the attributes from the database</returns>
        public Tour? GetTourById(int id)
        {
            Tour? tour;

            using (connection = new NpgsqlConnection(connectionString))
            {
                tour = connection.Query<Tour>("SELECT * FROM tours WHERE id = @id LIMIT 1", new { id = id }).ToList().FirstOrDefault();
            }
            return tour;
        }

        /// <summary>
        ///     Retrieves all tours
        /// </summary>
        /// <returns>List of all tours</returns>
        public List<Tour> GetTours()
        {
            List<Tour> tours;

            using (connection = new NpgsqlConnection(connectionString))
            {
                tours = connection.Query<Tour>("SELECT * FROM tours ORDER BY id ASC").ToList();
            }
            return tours;
        }

        /// <summary>
        ///     Updates the given tour object
        /// </summary>
        /// <param name="tour">Tour object to update</param>
        public void UpdateTour(Tour tour)
        {
            // Check required attributes
            try
            {
                CheckTourRequiredAttributes(tour);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            // Update tour
            using (connection = new NpgsqlConnection(connectionString))
            {
                string query = """
                    UPDATE tours SET name = @Name, description = @Description, from_location = @From_location, to_location = @To_location, transportation_type = @Transportation_type, 
                    distance = @Distance, estimated_time = @Estimated_time, route_information = @Route_information WHERE id = @Id
                    """;
                connection.Query<Tour>(query, tour);
            }
        }

        /// <summary>
        ///     Deletes the tour with the given id
        /// </summary>
        /// <param name="id">ID of the tour</param>
        public void DeleteTour(int id)
        {
            // Delete all tour logs of tour
            foreach (TourLog tourlog in GetTourLogsOfTour(id))
            {
                DeleteTourLog(tourlog.Id);
            }

            // Delete tour
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Query("DELETE FROM tours WHERE id = @id", new { id = id });
            }
        }

        /// <summary>
        ///     Checks all the required attributes of the given tour
        /// </summary>
        /// <param name="tour">Tour object to check</param>
        /// <returns>Exception, if required attribute of tour is missing or invalid</returns>
        public void CheckTourRequiredAttributes(Tour tour)
        {
            if (string.IsNullOrEmpty(tour.Name)) throw new Exception("Name is empty");
            if (string.IsNullOrEmpty(tour.Description)) throw new Exception("Description is empty");
            if (string.IsNullOrEmpty(tour.From_Location)) throw new Exception("From_Location is empty");
            if (string.IsNullOrEmpty(tour.To_Location)) throw new Exception("To_Location is empty");
            if (tour.Transportation_Type < 1) throw new Exception("Invalid Transportation_Type");
            if (tour.Distance < 1) throw new Exception("Distance must be a positive number");
            if (tour.Estimated_Time < 1) throw new Exception("Estimated_Time must be a positive number");
            if (string.IsNullOrEmpty(tour.Route_Information)) throw new Exception("Route_Information is empty");
        }

        /* Tour logs */

        /// <summary>
        ///     Creates a tour log with the given object
        /// </summary>
        /// <param name="tourlog">Object of the tourlog to be created</param>
        public void CreateTourLog(TourLog tourlog)
        {
            // Check required attributes
            try
            {
                CheckTourLogRequiredAttributes(tourlog);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            // Create tourlog
            using (connection = new NpgsqlConnection(connectionString))
            {
                string query = """
                    INSERT INTO tour_logs (tour_id, logdate, comment, difficulty, total_distance, total_time, rating)
                    VALUES (@Tour_id, @Logdate, @Comment, @Difficulty, @Total_distance, @Total_time, @Rating)
                    """;
                connection.Query<TourLog>(query, tourlog);
            }
        }

        /// <summary>
        ///     Retrieves the tour logs of the tour with the given id
        /// </summary>
        /// <param name="tourID">ID of the tour</param>
        /// <returns>List of tour logs</returns>
        public List<TourLog> GetTourLogsOfTour(int tourID)
        {
            List<TourLog> tourlogs = new List<TourLog>();

            using (connection = new NpgsqlConnection(connectionString))
            {
                tourlogs = connection.Query<TourLog>("SELECT * FROM tour_logs WHERE tour_id = @tour_id ORDER BY id ASC", new { tour_id = tourID }).ToList<TourLog>();
            }
            return tourlogs;
        }

        /// <summary>
        ///     Updates the tour log with the given id
        /// </summary>
        /// <param name="tourlog">Tour log object to update</param>
        public void UpdateTourLog(TourLog tourlog)
        {
            // Check required attributes
            try
            {
                CheckTourLogRequiredAttributes(tourlog);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            // Update tourlog
            using (connection = new NpgsqlConnection(connectionString))
            {
                string query = """
                    UPDATE tour_logs SET tour_id = @Tour_id, logdate = @Logdate, comment = @Comment, difficulty = @Difficulty, 
                    total_distance = @Total_distance, total_time = @Total_time, rating = @Rating WHERE id = @Id
                    """;
                connection.Query<Tour>(query, tourlog);
            }
        }

        /// <summary>
        ///     Deletes the tour log with the given id
        /// </summary>
        /// <param name="tourlogID">ID of the tour log</param>
        public void DeleteTourLog(int id)
        {
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Query("DELETE FROM tour_logs WHERE id = @id", new { id = id });
            }
        }

        /// <summary>
        ///     Checks all the required attributes of the given tour log
        /// </summary>
        /// <param name="tourlog">Tour log object to check</param>
        /// <returns>Exception, if required attribute of tour log is missing or invalid</returns>
        public void CheckTourLogRequiredAttributes(TourLog tourlog)
        {
            if (tourlog.Tour_Id < 1) throw new Exception("Invalid Tour ID");
            if (!(tourlog.Logdate is DateTime)) throw new Exception("Invalid DateTime");
            if (string.IsNullOrEmpty(tourlog.Comment)) throw new Exception("Comment is empty");
            if (tourlog.Difficulty < 1) throw new Exception("Invalid Difficulty");
            if (tourlog.Total_Distance < 1) throw new Exception("Total distance must be a positive number");
            if (tourlog.Total_Time < 1) throw new Exception("Total time must be a positive number");
            if (tourlog.Rating < 1 || tourlog.Rating > 10) throw new Exception("Rating must be between 1 and 10");
        }
    }
}
