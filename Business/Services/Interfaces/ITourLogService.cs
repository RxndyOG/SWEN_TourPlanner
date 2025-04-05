using Model;

namespace Business.Services.Interfaces
{
    public interface ITourLogService
    {
        List<TourLog> GetTourLogs(int tourID);
        void AddTourLog(TourLog tourlog);
        void DeleteTourLog(int id);
        void UpdateTourLog(TourLog tourlog);
    }
}
