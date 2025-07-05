using DataAccess.Repositories.Interfaces;
using DataAccess.Exceptions;
using DataAccess.Database;
using Model;
using Npgsql;
using Dapper;

namespace DataAccess.Repositories
{
    public class TourLogRepository : ITourLogRepository
    {
        private readonly IDatabase _db;
        private NpgsqlConnection? connection;

        public TourLogRepository(IDatabase db)
        {
            _db = db;
        }

        public List<TourLog> GetTourLogs(int tourID)
        {
            try 
            {
                List<TourLog> tourlogs = new List<TourLog>();

                using (connection = new NpgsqlConnection(_db.GetConnectionString()))
                {
                    tourlogs = connection.Query<TourLog>("SELECT * FROM tour_logs WHERE tour_id = @tour_id ORDER BY id ASC", new { tour_id = tourID }).ToList<TourLog>();
                }
                return tourlogs;
            }
            catch (Exception ex)
            {
                throw new FailedDatabaseConnectionException();
            }
        }

        public void AddTourLog(TourLog tourlog)
        {
            try 
            { 
                using (connection = new NpgsqlConnection(_db.GetConnectionString()))
                {
                    string query = """
                        INSERT INTO tour_logs (tour_id, logdate, comment, difficulty, total_distance, total_time, rating)
                        VALUES (@Tour_id, @Logdate, @Comment, @Difficulty, @Total_distance, @Total_time, @Rating)
                        """;
                    connection.Query<TourLog>(query, tourlog);
                }
            }
            catch (Exception ex)
            {
                throw new FailedDatabaseConnectionException();
            }
        }

        public void DeleteTourLog(int id)
        {
            try 
            {
                using (connection = new NpgsqlConnection(_db.GetConnectionString()))
                {
                    connection.Query("DELETE FROM tour_logs WHERE id = @id", new { id });
                }
            }
            catch (Exception ex)
            {
                throw new FailedDatabaseConnectionException();
            }
        }

        public void UpdateTourLog(TourLog tourlog)
        {
            try 
            {
                using (connection = new NpgsqlConnection(_db.GetConnectionString()))
                {
                    string query = """
                        UPDATE tour_logs SET tour_id = @Tour_id, logdate = @Logdate, comment = @Comment, difficulty = @Difficulty, 
                        total_distance = @Total_distance, total_time = @Total_time, rating = @Rating WHERE id = @Id
                        """;
                    connection.Query<Tour>(query, tourlog);
                }
            }
            catch (Exception ex)
            {
                throw new FailedDatabaseConnectionException();
            }
        }
    }
}
