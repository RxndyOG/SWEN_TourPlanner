using Business.Services.Interfaces;
using Business.Exceptions;
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
                throw;
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
                throw;
            }

            _tourRepository.UpdateTour(tour);
        }

        public void CheckTourRequiredAttributes(Tour tour)
        {
            if (string.IsNullOrEmpty(tour.Name)) throw new EmptyAttributeException("Name");
            if (string.IsNullOrEmpty(tour.Description)) throw new EmptyAttributeException("Description");
            if (string.IsNullOrEmpty(tour.From_Location)) throw new EmptyAttributeException("From_Location");
            if (string.IsNullOrEmpty(tour.To_Location)) throw new EmptyAttributeException("To_Location");
            if (string.IsNullOrEmpty(tour.Transportation_Type)) throw new EmptyAttributeException("Transportation_Type");
            if (string.IsNullOrEmpty(tour.Route_Information)) throw new EmptyAttributeException("Route_Information");

            if (tour.Distance < 1) throw new NonPositiveNumberException("Distance");
            if (tour.Estimated_Time < 1) throw new NonPositiveNumberException("Estimated_Time");
        }
    }
}
