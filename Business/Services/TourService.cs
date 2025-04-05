using Business.Services.Interfaces;
using DataAccess.Repositories.Interfaces;
using Model;

namespace Business.Services
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;

        public TourService(ITourRepository tourRepository) 
        {
            _tourRepository = tourRepository;
        }

        public Tour GetTour(int id)
        {
            return _tourRepository.GetTour(id);
        }

        public List<Tour> GetAllTours()
        {
            return _tourRepository.GetAllTours();
        }

        public void AddTour(Tour tour)
        {
            try
            {
                CheckTourRequiredAttributes(tour);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            _tourRepository.AddTour(tour);
        }

        public void DeleteTour(int id)
        {
            _tourRepository.DeleteTour(id);
        }

        public void UpdateTour(Tour tour)
        {
            try
            {
                CheckTourRequiredAttributes(tour);
            }
            catch (Exception e) 
            { 
                throw new Exception(e.Message);
            }

            _tourRepository.UpdateTour(tour);
        }

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
    }
}
