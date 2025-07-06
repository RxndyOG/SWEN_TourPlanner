using Model;
namespace Business.Services.Interfaces
{
    public interface IReportService
    {
        void GeneratePdfReport(List<Tour> tours);
        void GeneratePdfReportSingleTour(Tour tour);
    }
}
