using Business.Services.Interfaces;
using DataAccess.Repositories.Interfaces;
using Model;

namespace Business.Services
{
    public class TourLogService : ITourLogService
    {
        private readonly ITourLogRepository _tourlogRepository;

        public TourLogService(ITourLogRepository tourlogrepository)
        {
            _tourlogRepository = tourlogrepository;
        }

        public List<TourLog> GetTourLogs(int tourID)
        {
            return _tourlogRepository.GetTourLogs(tourID);
        }

        public void AddTourLog(TourLog tourlog)
        {
            _tourlogRepository.AddTourLog(tourlog);
        }

        public void DeleteTourLog(int id)
        {
            _tourlogRepository.DeleteTourLog(id);
        }

        public void UpdateTourLog(TourLog tourlog)
        {
            _tourlogRepository.UpdateTourLog(tourlog);
        }

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
