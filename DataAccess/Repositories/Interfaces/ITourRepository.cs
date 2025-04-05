using Model;

namespace DataAccess.Repositories.Interfaces
{
    public interface ITourRepository
    {
        Tour GetTour(int id);
        List<Tour> GetAllTours();
        void AddTour(Tour tour);
        void DeleteTour(int id);
        void UpdateTour(Tour tour);
    }
}
