using Accessibility;
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

        public TourManagementViewModel(NavigationViewModel nv, MainViewModel mv)
        {
            _navigationViewModel = nv;
            _mainViewModel = mv;
            ReportCommand = new RelayCommand(GeneratePdfReport);

            SaveTourCommand = new RelayCommand(SaveTour);
            ImportTourCommand = new RelayCommand(ImportTour);
            DeleteCommand = new RelayCommand(DeleteTour);
            ModifyCommand = new RelayCommand(ModifyTour);
            ExportCommand = new RelayCommand(ExportTour);
            RemoveBlockCommand = new RelayCommand(RemoveBlock);

            ShowMapCommand = new RelayCommand(_ => ShowMapAndCaptureImage());

            if (GlobalFontSettings.FontResolver == null)
                GlobalFontSettings.FontResolver = new SimpleFontResolver();
        }

        public class SimpleFontResolver : IFontResolver
        {
            private static readonly string FontName = "LiberationSans";
            private static readonly string FontFile = "Fonts/LiberationSans-Regular.ttf"; // Add this TTF to your project!

            public byte[] GetFont(string faceName)
            {
                return File.ReadAllBytes("Views/Images/LiberationSans-Regular.ttf");
            }

            public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
            {
                return new FontResolverInfo(FontName);
            }
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
                            gfx.DrawString($"From: {tour.From}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                            gfx.DrawString($"To: {tour.To}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                            gfx.DrawString($"Transport: {tour.Transport}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                            gfx.DrawString($"Distance: {tour.Distance}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                            gfx.DrawString($"Description: {tour.Description}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                            gfx.DrawString($"Route Info: {tour.RouteInfo}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                            gfx.DrawString($"Estimated Time: {tour.EstimatedTime}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 30;

                            if (tour.TourLog != null && tour.TourLog.TourLogsTable != null && tour.TourLog.TourLogsTable.Count > 0)
                            {
                                gfx.DrawString("Tour Logs:", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                                foreach (var log in tour.TourLog.TourLogsTable)
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

        public void DeleteTour(object parameter)
        {

            if (parameter is int tourID)
            {
                log.Info($"DeleteTour called for TourID: {tourID}");

                _navigationViewModel.CurrentPageRight = new DeleteWindowNothingHere();
                if (tourID >= 0 && tourID < Tours.Count)
                {
                    var blockToRemove = Blocks.FirstOrDefault(b => b.TourID == tourID);

                    if (blockToRemove != null)
                    {
                        Blocks.Remove(blockToRemove);
                        log.Info($"Block with TourID {tourID} removed from Blocks.");

                        Console.WriteLine($"Block mit TourID {tourID} wurde entfernt.");
                        Tours.Remove(Tours[0]); // temp
                        var tourToRemove = Tours.FirstOrDefault(t => t.ID == tourID);
                        if (tourToRemove != null)
                        {
                            Tours.Remove(tourToRemove);
                            log.Info($"Tour with ID {tourID} removed from Tours.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Kein Block mit TourID {tourID} gefunden.");
                        log.Warn($"No block found with TourID {tourID}.");

                    }
                }
                else
                {
                    log.Warn($"DeleteTour: Invalid TourID {tourID}.");
                }
            }
        }

        public void ExportTour(object parameter)
        {
            if (parameter is AddTourModel tourModel)
            {
                log.Info($"ExportTour called for TourID: {tourModel.ID}");
                if (Tours == null || Tours.Count == 0)
                {
                    log.Warn("ExportTour aborted: Tour list is empty.");
                    MessageBox.Show("Die Tour-Liste ist leer!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (tourModel.ID < 0 || tourModel.ID >= Tours.Count)
                {
                    log.Warn($"ExportTour aborted: Invalid index {tourModel.ID}.");
                    MessageBox.Show($"Ungültiger Index: {tourModel.ID}. Maximaler Wert: {Tours.Count - 1}",
                                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JSON Datei (*.json)|*.json";
                saveFileDialog.Title = "Tour speichern";

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
                if (Tours == null || Tours.Count == 0)
                {
                    MessageBox.Show("Die Tour-Liste ist leer!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (tourModel.ID < 0 || tourModel.ID >= Tours.Count)
                {
                    MessageBox.Show($"Ungültiger Index: {tourModel.ID}. Maximaler Wert: {Tours.Count - 1}",
                                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var blockToRemove = Blocks.FirstOrDefault(b => b.TourID == tourModel.ID);

                if (blockToRemove != null)
                {
                    Blocks.Remove(blockToRemove);
                    Console.WriteLine($"Block mit TourID {tourModel.ID} wurde entfernt.");
                    Tours.Add(tourModel);
                    AddTour(tourModel);
                }
                else
                {
                    Console.WriteLine($"Kein Block mit TourID {tourModel.ID} gefunden.");
                }



                _navigationViewModel.CurrentPageRight = new DeleteWindowNothingHere();
            }
        }

        private void ImportTour(object parameter)
        {
            log.Info("ImportTour called.");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON Datei (*.json)|*.json";
            openFileDialog.Title = "Benutzerdaten laden";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string json = File.ReadAllText(openFileDialog.FileName);
                    AddTourModel tourModel = JsonSerializer.Deserialize<AddTourModel>(json);

                    if (tourModel != null)
                    {
                        tourModel.ID = Tours.Count;
                        Tours.Add(tourModel);
                        AddTour(tourModel);
                        log.Info($"Tour imported from {openFileDialog.FileName} as ID {tourModel.ID}");
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error importing tour.", ex);
                    MessageBox.Show("Fehler beim Laden der Datei:\n" + ex.Message);
                }
            }
        }

        private void SaveTour(object parameter)
        {
            Console.WriteLine("SaveTour aufgerufen");
            log.Info("SaveTour called.");
            if (string.IsNullOrWhiteSpace(NewTour.Name) || string.IsNullOrWhiteSpace(NewTour.From) || string.IsNullOrWhiteSpace(NewTour.To) || string.IsNullOrWhiteSpace(NewTour.Transport) || string.IsNullOrWhiteSpace(NewTour.Distance) || string.IsNullOrWhiteSpace(NewTour.Description) || string.IsNullOrWhiteSpace(NewTour.RouteInfo) || string.IsNullOrWhiteSpace(NewTour.EstimatedTime))
            {
                log.Warn("SaveTour aborted: Required input is missing.");

                MessageBox.Show("Required Input is Missing", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            AddTourModel Tour = new AddTourModel
            {
                ID = Tours.Count,
                Name = NewTour.Name,
                From = NewTour.From,
                To = NewTour.To,
                Description = NewTour.Description,
                Transport = NewTour.Transport,
                Distance = NewTour.Distance,
                EstimatedTime = NewTour.EstimatedTime,
                RouteInfo = NewTour.RouteInfo,
                ImagePath = NewTour.ImagePath
            };

            Tours.Add(Tour);

            AddTour(Tour);

            Console.WriteLine(Tour.ID);

            log.Info($"Tour saved: {Tour.Name} (ID: {Tour.ID})");

            NewTour = new AddTourModel();
        }

        private void AddTour(AddTourModel Tour)
        {
            var newBlock = new BlockModel
            {
                TourID = Tour.ID,
                Description2 = Tour.Description,
                Text = Tour.Name,
                RemoveCommand = RemoveBlockCommand,
                NavigateCommandRight = _navigationViewModel.NavigateCommandRight
            };

            Blocks.Add(newBlock);
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
            var from = NewTour.From;
            var to = NewTour.To;

            System.Diagnostics.Debug.WriteLine($"FROM: {from}, TO: {to}");

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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
