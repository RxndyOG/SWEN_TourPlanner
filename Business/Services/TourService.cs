using Business.Services.Interfaces;
using Business.Exceptions;
using DataAccess.Repositories.Interfaces;
using Model;
using Microsoft.Win32;
using System.Text.Json;
using System.Text.Json.Nodes;
using log4net;

namespace Business.Services
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;
        private static readonly ILog log = LogManager.GetLogger(typeof(TourService));

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

            if (tour.Distance < 1) throw new NonPositiveNumberException("Distance");
            if (tour.Estimated_Time < 1) throw new NonPositiveNumberException("Estimated_Time");
        }

        public Tour ImportTour(TourLogService tourLogService)
        {
            log.Info("ImportTour called.");
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Datei (*.json)|*.json",
                Title = "Import Tour"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                JsonArray jsonObject = JsonSerializer.Deserialize<JsonArray>(json);

                Tour tour = JsonSerializer.Deserialize<Tour>(jsonObject[0]);
                if (tour != null)
                {
                    // Save tour in database
                    AddTour(tour);

                    // Get new ID after saving
                    List<Tour> allTours = GetAllTours();
                    Tour savedTour = allTours.OrderByDescending(t => t.Id).FirstOrDefault();
                    if (savedTour != null)
                    {
                        tour.Id = savedTour.Id;

                        // Get Array of TourLogs
                        TourLog[] tourLogArray = JsonSerializer.Deserialize<TourLog[]>(jsonObject[1]) ?? [];

                        // Save imported TourLogs to the database
                        foreach (TourLog tourLog in tourLogArray)
                        {

                            var dbLog = new TourLog
                            {
                                Tour_Id = tour.Id,
                                Logdate = DateTime.TryParse($"{tourLog.Logdate}", out var dt) ? dt : DateTime.Now,
                                Comment = tourLog.Comment,
                                Difficulty = tourLog.Difficulty,
                                Total_Distance = int.TryParse((tourLog.Total_Distance).ToString(), out var dist) ? dist : 0,
                                Total_Time = int.TryParse((tourLog.Total_Time).ToString(), out var dur) ? dur : 0,
                                Rating = int.TryParse((tourLog.Rating).ToString(), out var rat) ? rat : 1
                            };

                            tourLogService.AddTourLog(dbLog);
                        }
                    }
                    return tour;
                }
            }

            throw new Exception("Failed to open File");
        }

        public void ExportTour(Tour tour, TourLogService tourLogService)
        {
            log.Info($"ExportTour called for TourID: {tour.Id}");

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON Datei (*.json)|*.json",
                Title = "Export Tour"
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                throw new Exception("Error while exporting Tour");
            }

            List<TourLog> tourlogs = tourLogService.GetTourLogs(tour.Id);

            JsonArray jsonArray = new JsonArray();
            jsonArray.Add(tour);
            jsonArray.Add(tourlogs);

            string json_together = JsonSerializer.Serialize(jsonArray, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(saveFileDialog.FileName, json_together);
            log.Info($"Tour exported to {saveFileDialog.FileName}");
        }
    }
}
