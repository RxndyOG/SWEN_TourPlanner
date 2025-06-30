using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;
using UI.Commands;
using Model;
using log4net;
using UI.Views;
using Accessibility;

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

        public ICommand SaveTourCommand { get; }
        public ICommand ImportTourCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ModifyCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand RemoveBlockCommand { get; }

        private MainViewModel _mainViewModel;
        private NavigationViewModel _navigationViewModel;

        public TourManagementViewModel(NavigationViewModel nv, MainViewModel mv)
        {
            _navigationViewModel = nv;
            _mainViewModel = mv;


            SaveTourCommand = new RelayCommand(SaveTour);
            ImportTourCommand = new RelayCommand(ImportTour);
            DeleteCommand = new RelayCommand(DeleteTour);
            ModifyCommand = new RelayCommand(ModifyTour);
            ExportCommand = new RelayCommand(ExportTour);
            RemoveBlockCommand = new RelayCommand(RemoveBlock);
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
