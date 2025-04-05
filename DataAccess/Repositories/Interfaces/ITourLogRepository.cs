using Model;

namespace DataAccess.Repositories.Interfaces
{
    public interface ITourLogRepository
    {
        List<TourLog> GetTourLogs(int tourID);
        void AddTourLog(TourLog tourlog);
        void DeleteTourLog(int id);
        void UpdateTourLog(TourLog tourlog);
    }
}
