using DataAccess.Repositories.Interfaces;
using DataAccess.Exceptions;
using DataAccess.Database;
using Model;
using Npgsql;
using Dapper;
using log4net;

namespace DataAccess.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly IDatabase _db;
        private NpgsqlConnection? connection;
        private static readonly ILog log = LogManager.GetLogger(typeof(TourRepository));

        public TourRepository(IDatabase db)
        {
            _db = db;
        }

        public Tour GetTour(int id)
        {
            try
            {
                Tour? tour;

                using (connection = new NpgsqlConnection(_db.GetConnectionString()))
                {
                    tour = connection.Query<Tour>("SELECT * FROM tours WHERE id = @id LIMIT 1", new { id }).ToList().FirstOrDefault();
                }

                log.Info($"[Database] Retrieved Tour with id: {id}");
                return tour;
            }
            catch (Exception ex)
            {
                log.Error("[Database] ", ex);
                throw new DatabaseException(ex.Message);
            }
        }

        public List<Tour> GetAllTours()
        {
            try
            {
                List<Tour> tours;

                using (connection = new NpgsqlConnection(_db.GetConnectionString()))
                {
                    tours = connection.Query<Tour>("SELECT * FROM tours ORDER BY id ASC").ToList();
                }

                log.Info($"[Database] Retrieved {tours.Count} Tours");
                return tours;
            }
            catch (Exception ex)
            {
                log.Error("[Database]", ex);
                throw new DatabaseException(ex.Message);
            }
        }

        public void AddTour(Tour tour)
        {
            try 
            {
                using (connection = new NpgsqlConnection(_db.GetConnectionString()))
                {
                    string query = """
                        INSERT INTO tours (name, description, from_location, to_location, transportation_type, distance, estimated_time, route_information)
                        VALUES (@Name, @Description, @From_location, @To_location, @Transportation_type, @Distance, @Estimated_time, @Route_information)
                        """;
                    connection.Query<Tour>(query, tour);
                }
                log.Info($"[Database] Added Tour");
            }
            catch (Exception ex)
            {
                log.Error("[Database]", ex);
                throw new DatabaseException(ex.Message);
            }
        }

        public void DeleteTour(int id)
        {
            try
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
                log.Info($"[Database] Deleted Tour with id: {id}");
            }
            catch (Exception ex)
            {
                log.Error("[Database]", ex);
                throw new DatabaseException(ex.Message);
            }
        }

        public void UpdateTour(Tour tour)
        {
            try {
                using (connection = new NpgsqlConnection(_db.GetConnectionString()))
                {
                    string query = """
                        UPDATE tours SET name = @Name, description = @Description, from_location = @From_location, to_location = @To_location, transportation_type = @Transportation_type, 
                        distance = @Distance, estimated_time = @Estimated_time, route_information = @Route_information WHERE id = @Id
                        """;
                    connection.Query<Tour>(query, tour);
                }
                log.Info($"[Database] Updated Tour with id: {tour.Id}");
            }
            catch (Exception ex)
            {
                log.Error("[Database]", ex);
                throw new DatabaseException(ex.Message);
            }
        }
    }
}
