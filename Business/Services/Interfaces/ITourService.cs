using Model;

namespace Business.Services.Interfaces
{
    public interface ITourService
    {
        Tour GetTour(int id);
        List<Tour> GetAllTours();
        void AddTour(Tour tour);
        void DeleteTour(int id);
        void UpdateTour(Tour tour);
        Tour ImportTour(TourLogService tourLogService);
        void ExportTour(Tour tour, TourLogService tourLogService);
    }
}
