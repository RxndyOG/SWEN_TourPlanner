using Accessibility;
using Business.Services;
using Business.Services;
using Business.Services.Interfaces;
using DataAccess.Database;
using DataAccess.Repositories;
using DataAccess.Repositories.Interfaces;
using log4net;
using Microsoft.Win32;
using Model;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Quality;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using UI.Commands;
using UI.Views;


namespace UI.ViewModels
{
    public class TourManagementViewModel : INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TourManagementViewModel));

        public ObservableCollection<AddTourModel> Tours { get; set; } = new();
        public ObservableCollection<BlockModel> Blocks { get; set; } = new();

        private AddTourModel _newTour = new AddTourModel();
        public AddTourModel NewTour
        {
            get => _newTour;
            set { _newTour = value; OnPropertyChanged(nameof(NewTour)); }
        }

        public ICommand ReportCommand { get; }
        public ICommand SaveTourCommand { get; }
        public ICommand ImportTourCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ModifyCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand RemoveBlockCommand { get; }
        public ICommand ShowMapCommand { get; }

        private MainViewModel _mainViewModel;
        private NavigationViewModel _navigationViewModel;
        private TourService _tourService;
        private TourLogService _tourLogService;

        public TourManagementViewModel(NavigationViewModel nv, MainViewModel mv)
        {
            _tourService = new TourService(new TourRepository(new Database()));
            _tourLogService = new TourLogService(new TourLogRepository(new Database()));
            _navigationViewModel = nv;
            _mainViewModel = mv;

            Tours = new ObservableCollection<AddTourModel>(
        _tourService.GetAllTours().Select(tour =>
        {
            var model = new AddTourModel
            {
                Id = tour.Id,
                Name = tour.Name,
                Description = tour.Description,
                From_Location = tour.From_Location,
                To_Location = tour.To_Location,
                Transportation_Type = tour.Transportation_Type,
                Distance = tour.Distance,
                Estimated_Time = tour.Estimated_Time,
                Route_Information = tour.Route_Information,
            };

            // TourLogs aus DB laden
            var logs = _tourLogService.GetTourLogs(tour.Id);
            foreach (var log in logs)
            {
                model.TourLogsTable.Add(new TourLogs.TourLog
                {
                    IDTourLogs = log.Id,
                    Date = log.Logdate.ToShortDateString(),
                    Time = log.Logdate.ToShortTimeString(),
                    Comment = log.Comment,
                    Difficulty = log.Difficulty.ToString(),
                    Distance = log.Total_Distance.ToString(),
                    Duration = log.Total_Time.ToString(),
                    Rating = log.Rating.ToString()
                });
            }
            return model;
        })
    );
            foreach (var tour in Tours)
            {
                AddTourBlock(tour);
            }

            ReportCommand = new RelayCommand(GeneratePdfReport);
            SaveTourCommand = new RelayCommand(SaveTour);
            ImportTourCommand = new RelayCommand(ImportTour);
            DeleteCommand = new RelayCommand(DeleteTour);
            ModifyCommand = new RelayCommand(ModifyTour);
            ExportCommand = new RelayCommand(ExportTour);
            RemoveBlockCommand = new RelayCommand(RemoveBlock);
            ShowMapCommand = new RelayCommand(_ => ShowMapAndCaptureImage());
        }

        private void SaveTour(object parameter)
        {
            log.Info("SaveTour called.");
   
            if (string.IsNullOrWhiteSpace(NewTour.Name) ||
                string.IsNullOrWhiteSpace(NewTour.From_Location) ||
                string.IsNullOrWhiteSpace(NewTour.To_Location) ||
                string.IsNullOrWhiteSpace(NewTour.Description) ||
                string.IsNullOrWhiteSpace(NewTour.Route_Information) ||
                string.IsNullOrWhiteSpace(NewTour.Transportation_Type) ||
                NewTour.Distance <= 0 ||
                NewTour.Estimated_Time <= 0)
            {
                MessageBox.Show("Required Input is Missing", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                log.Warn("SaveTour aborted: Required input is missing.");
                return;
            }


   
            var tourModel = new AddTourModel
            {
                Name = NewTour.Name,
                From_Location = NewTour.From_Location,
                To_Location = NewTour.To_Location,
                Description = NewTour.Description,
                Transportation_Type = NewTour.Transportation_Type,
                Distance = NewTour.Distance,
                Estimated_Time = NewTour.Estimated_Time,
                Route_Information = NewTour.Route_Information,
                ImagePath = NewTour.ImagePath
            };


            _tourService.AddTour(tourModel.Tour);

            var allTours = _tourService.GetAllTours();
            var savedTour = allTours.OrderByDescending(t => t.Id).FirstOrDefault();

            if (savedTour != null)
            {
                tourModel.Id = savedTour.Id;
            }

            Tours.Add(tourModel);
            AddTourBlock(tourModel);

            log.Info($"Tour saved: {tourModel.Name} (ID: {tourModel.Id})");

            NewTour = new AddTourModel();
        }

        private void AddTourBlock(AddTourModel tour)
        {
            var newBlock = new BlockModel
            {
                TourID = tour.Id,
                Description2 = tour.Description,
                Text = tour.Name,
                RemoveCommand = RemoveBlockCommand,
                NavigateCommandRight = _navigationViewModel.NavigateCommandRight
            };
            Blocks.Add(newBlock);
        }

        public void DeleteTour(object parameter)
        {
            if (parameter is int tourID)
            {
                // Erst aus der Datenbank löschen
                _tourService.DeleteTour(tourID);

                log.Info($"DeleteTour called for TourID: {tourID}");
                _navigationViewModel.CurrentPageRight = new DeleteWindowNothingHere();

                // Aus der UI-Collection entfernen
                var blockToRemove = Blocks.FirstOrDefault(b => b.TourID == tourID);
                if (blockToRemove != null)
                {
                    Blocks.Remove(blockToRemove);
                    log.Info($"Block with TourID {tourID} removed from Blocks.");
                }

                var tourToRemove = Tours.FirstOrDefault(t => t.Id == tourID);
                if (tourToRemove != null)
                {
                    Tours.Remove(tourToRemove);
                    log.Info($"Tour with ID {tourID} removed from Tours.");
                }
                else
                {
                    log.Warn($"No tour found with ID {tourID}.");
                }
            }
            else if (parameter is AddTourModel tourModel)
            {
                // Falls das Command ein AddTourModel übergibt
                _tourService.DeleteTour(tourModel.Id);

                var blockToRemove = Blocks.FirstOrDefault(b => b.TourID == tourModel.Id);
                if (blockToRemove != null)
                    Blocks.Remove(blockToRemove);

                if (Tours.Contains(tourModel))
                    Tours.Remove(tourModel);

                log.Info($"Tour with ID {tourModel.Id} removed via AddTourModel parameter.");
            }
        }

        public void ExportTour(object parameter)
        {
            if (parameter is AddTourModel tourModel)
            {
                log.Info($"ExportTour called for TourID: {tourModel.Id}");
                if (Tours == null || Tours.Count == 0)
                {
                    MessageBox.Show("Die Tour-Liste ist leer!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "JSON Datei (*.json)|*.json",
                    Title = "Tour speichern"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string json = JsonSerializer.Serialize(tourModel, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(saveFileDialog.FileName, json);
                    log.Info($"Tour exported to {saveFileDialog.FileName}");
                    MessageBox.Show("Tour Exported!");
                }
            }
        }

        public void ModifyTour(object parameter)
        {
            if (parameter is AddTourModel tourModel)
            {
                var existing = Tours.FirstOrDefault(t => t.Id == tourModel.Id);
                if (existing != null)
                {
                    Tours.Remove(existing);
                    Blocks.Remove(Blocks.FirstOrDefault(b => b.TourID == tourModel.Id));
                    Tours.Add(tourModel);
                    AddTourBlock(tourModel);

                    log.Info($"Tour with ID {tourModel.Id} modified.");

                    _tourService.UpdateTour(tourModel.Tour);

                }
                else
                {
                    MessageBox.Show("Tour nicht gefunden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void ImportTour(object parameter)
        {
            log.Info("ImportTour called.");
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Datei (*.json)|*.json",
                Title = "Benutzerdaten laden"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string json = File.ReadAllText(openFileDialog.FileName);
                    AddTourModel tourModel = JsonSerializer.Deserialize<AddTourModel>(json);

                    if (tourModel != null)
                    {
                        // Tour in die Datenbank speichern
                        _tourService.AddTour(tourModel.Tour);

                        // Nach dem Speichern die neue ID aus der DB holen
                        var allTours = _tourService.GetAllTours();
                        var savedTour = allTours.OrderByDescending(t => t.Id).FirstOrDefault();
                        if (savedTour != null)
                            tourModel.Id = savedTour.Id;

                        Tours.Add(tourModel);
                        AddTourBlock(tourModel);
                        log.Info($"Tour imported from {openFileDialog.FileName} as ID {tourModel.Id}");
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error importing tour.", ex);
                    MessageBox.Show("Fehler beim Laden der Datei:\n" + ex.Message);
                }
            }
        }

        private void RemoveBlock(object block)
        {
            if (block is BlockModel blockModel && Blocks.Contains(blockModel))
            {
                Blocks.Remove(blockModel);
            }
        }

        public void ShowMapAndCaptureImage()
        {
            var from = NewTour.From_Location;
            var to = NewTour.To_Location;

            Debug.WriteLine($"FROM: {from}, TO: {to}");

            var mapWindow = new Window
            {
                Title = "Karte auswählen",
                Content = new MapCaptureControl(this),
                Width = 600,
                Height = 450
            };

            mapWindow.Loaded += (s, e) =>
            {
                if (mapWindow.Content is MapCaptureControl mapControl)
                {
                    mapControl.SetRoute(from, to);
                }
            };

            if (mapWindow.Content is MapCaptureControl mapControl2)
            {
                mapControl2.MapImageSaved += (filePath) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NewTour.ImagePath = filePath;
                        OnPropertyChanged(nameof(NewTour));
                        mapWindow.Close();
                    });
                };
            }

            mapWindow.ShowDialog();
        }


        public void GeneratePdfReport(object parameter)
        {
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    Title = "Save PDF Report"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    if (string.IsNullOrWhiteSpace(filePath))
                    {
                        MessageBox.Show("Invalid file path selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    PdfDocument document = new PdfDocument();
                    document.Info.Title = "Tour Report";

                    PdfPage page = document.AddPage();
                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    var titleFont = new XFont("LiberationSans", 20, XFontStyleEx.Regular);
                    var contentFont = new XFont("LiberationSans", 12, XFontStyleEx.Regular);

                    double y = 40;
                    gfx.DrawString("Tour Report", titleFont, XBrushes.Black, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
                    y += 40;

                    if (Tours != null && Tours.Count > 0)
                    {
                        foreach (var tour in Tours)
                        {
                            gfx.DrawString($"Name: {tour.Name}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                            gfx.DrawString($"From: {tour.From_Location}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                            gfx.DrawString($"To: {tour.To_Location}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                            gfx.DrawString($"Transport: {tour.Transportation_Type}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                            gfx.DrawString($"Distance: {tour.Distance}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                            gfx.DrawString($"Description: {tour.Description}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                            gfx.DrawString($"Route Info: {tour.Route_Information}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                            gfx.DrawString($"Estimated Time: {tour.Estimated_Time}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 30;

                            if (tour.TourLogsTable != null && tour.TourLogsTable != null && tour.TourLogsTable.Count > 0)
                            {
                                gfx.DrawString("Tour Logs:", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                                foreach (var log in tour.TourLogsTable)
                                {
                                    gfx.DrawString($"  - Date: {log.Date}, Time: {log.Time}, Difficulty: {log.Difficulty}, Distance: {log.Distance}, Comment: {log.Comment}, Duration: {log.Duration}", contentFont, XBrushes.Black, new XPoint(50, y));
                                    y += 20;
                                }
                                y += 10;
                            }
                            y += 10;
                            if (y > page.Height - 60)
                            {
                                page = document.AddPage();
                                gfx = XGraphics.FromPdfPage(page);
                                y = 40;
                            }
                        }
                    }
                    else
                    {
                        gfx.DrawString("No tours available.", contentFont, XBrushes.Black, new XPoint(40, y));
                    }

                    document.Save(filePath);

                    MessageBox.Show("PDF report generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error generating PDF report", ex);
                MessageBox.Show($"An error occurred while generating the PDF report:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
