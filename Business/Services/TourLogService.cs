using Business.Services.Interfaces;
using Business.Exceptions;
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
            try
            {
                CheckTourLogRequiredAttributes(tourlog);
            }
            catch (Exception e)
            {
                throw;
            }

            _tourlogRepository.AddTourLog(tourlog);
        }

        public void DeleteTourLog(int id)
        {
            _tourlogRepository.DeleteTourLog(id);
        }

        public void UpdateTourLog(TourLog tourlog)
        {
            try
            {
                CheckTourLogRequiredAttributes(tourlog);
            }
            catch (Exception e)
            {
                throw;
            }

            _tourlogRepository.UpdateTourLog(tourlog);
        }

        public void CheckTourLogRequiredAttributes(TourLog tourlog)
        {
            if (tourlog.Tour_Id < 1) throw new NonPositiveNumberException("Tour_Id");
            if (tourlog.Total_Distance < 1) throw new NonPositiveNumberException("Total_Distance");
            if (tourlog.Total_Time < 1) throw new NonPositiveNumberException("Total_Time");

            if (string.IsNullOrEmpty(tourlog.Comment)) throw new EmptyAttributeException("Comment");
            if (string.IsNullOrEmpty(tourlog.Difficulty)) throw new EmptyAttributeException("Difficulty");

            if (!(tourlog.Logdate is DateTime)) throw new Exception("Value of Logdate must be of type DateTime");
            if (tourlog.Rating < 1 || tourlog.Rating > 10) throw new Exception("Value of Rating must be between 1 and 10");
        }
    }
}
