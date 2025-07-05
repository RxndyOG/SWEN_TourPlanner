using DataAccess.Repositories.Interfaces;
using DataAccess.Database;
using Model;
using Npgsql;
using Dapper;

namespace DataAccess.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly IDatabase _db;
        private NpgsqlConnection? connection;

        public TourRepository(IDatabase db)
        {
            _db = db;
        }

        public Tour GetTour(int id)
        {
            Tour? tour;

            using (connection = new NpgsqlConnection(_db.GetConnectionString()))
            {
                tour = connection.Query<Tour>("SELECT * FROM tours WHERE id = @id LIMIT 1", new { id }).ToList().FirstOrDefault();
            }
            return tour;
        }

        public List<Tour> GetAllTours()
        {
            List<Tour> tours;

            using (connection = new NpgsqlConnection(_db.GetConnectionString()))
            {
                tours = connection.Query<Tour>("SELECT * FROM tours ORDER BY id ASC").ToList();
            }
            return tours;
        }

        public void AddTour(Tour tour)
        {
            using (connection = new NpgsqlConnection(_db.GetConnectionString()))
            {
                string query = """
                    INSERT INTO tours (name, description, from_location, to_location, transportation_type, distance, estimated_time, route_information, image_path)
                    VALUES (@Name, @Description, @From_location, @To_location, @Transportation_type, @Distance, @Estimated_time, @Route_information, @Image_Path)
                    """;
                connection.Query<Tour>(query, tour);
            }
        }

        public void DeleteTour(int id)
        {
            // Delete all tour logs of tour
            TourLogRepository tourLogRepository = new TourLogRepository(_db);
            foreach (TourLog tourlog in tourLogRepository.GetTourLogs(id))
            {
                tourLogRepository.DeleteTourLog(tourlog.Id);
            }

            // Delete tour
            using (connection = new NpgsqlConnection(_db.GetConnectionString()))
            {
                connection.Query("DELETE FROM tours WHERE id = @id", new { id });
            }
        }

        public void UpdateTour(Tour tour)
        {
            // Update tour
            using (connection = new NpgsqlConnection(_db.GetConnectionString()))
            {
                string query = """
                    UPDATE tours SET name = @Name, description = @Description, from_location = @From_location, to_location = @To_location, transportation_type = @Transportation_type, 
                    distance = @Distance, estimated_time = @Estimated_time, route_information = @Route_information WHERE id = @Id
                    """;
                connection.Query<Tour>(query, tour);
            }
        }
    }
}
