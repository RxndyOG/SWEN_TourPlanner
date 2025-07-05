using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UI.Commands;
using UI.Views;
using Model;
using System.Text.Json;
using System.Windows.Data;
using log4net;

namespace UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(MainViewModel));

     

        private NavigationViewModel _navigationViewModel;
        public NavigationViewModel Navigation => _navigationViewModel;

        private TourManagementViewModel _tourManagementViewModel;
        public TourManagementViewModel TourManagement => _tourManagementViewModel;

        private PropertyChangedEventHandler? _propertyChangedEventHandler;

        //public ICommand ShowMapCommand { get; }

        private BitmapImage _userUploadedImage;

        public ICommand UploadImageCommand { get; }


        public MainViewModel()
        {

            //ShowMapCommand = new RelayCommand(_ => ShowMapAndCaptureImage());

            _navigationViewModel = new NavigationViewModel(this);
            _tourManagementViewModel = new TourManagementViewModel(_navigationViewModel, this);

            log.Info("MainViewModel Konstrukture run start <UI.ViewModels.MainViewModel>");

            FilteredBlocks = CollectionViewSource.GetDefaultView(_tourManagementViewModel.Blocks);
            FilteredBlocks.Filter = FilterBlocks;
        }
      

        private string _searchText;
        private ICollectionView _filteredBlocks;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    FilteredBlocks.Refresh();
                }
            }
        }

        public ICollectionView FilteredBlocks
        {
            get => _filteredBlocks;
            private set
            {
                _filteredBlocks = value;
                OnPropertyChanged();
            }
        }
     
        public BitmapImage UserUploadedImage
        {
            get { return _userUploadedImage; }
            set
            {
                _userUploadedImage = value;
                OnPropertyChanged(nameof(UserUploadedImage));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                Debug.Print($"added PropertyChanged-handler {value}");
                _propertyChangedEventHandler += value;
            }
            remove
            {
                Debug.Print($"removed PropertyChanged-handler {value}");
                _propertyChangedEventHandler -= value;
            }
        }
       
        /*
        public void ShowMapAndCaptureImage()
        {
            var mapWindow = new Window
            {
                Title = "Karte auswählen",
                Content = new MapCaptureControl(_tourManagementViewModel),
                Width = 600,
                Height = 450
            };

            if (mapWindow.Content is MapCaptureControl mapControl)
            {
                mapControl.MapImageSaved += (filePath) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _tourManagementViewModel.NewTour.ImagePath = filePath;
                        OnPropertyChanged(nameof(_tourManagementViewModel.NewTour));
                        mapWindow.Close();
                    });
                };
            }

            mapWindow.ShowDialog();
        }
        */
        private void UploadImageFunc(object parameter)
        {
            log.Info("UploadImageFunc called.");
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = "Image files (*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png",
                FilterIndex = 1
            };

            if (openDialog.ShowDialog() == true)
            {
                string projectRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", ".."));
                string destinationFolder = Path.Combine(projectRoot, "UserImages");

                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                    log.Info($"Created directory: {destinationFolder}");
                }

                string uniqueFileName = $"{Path.GetFileNameWithoutExtension(openDialog.FileName)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(openDialog.FileName)}";
                string destinationPath = Path.Combine(destinationFolder, uniqueFileName);
                File.Copy(openDialog.FileName, destinationPath);

                log.Info($"Image uploaded: {destinationPath}");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _tourManagementViewModel.NewTour.Image_Path = destinationPath;
                    OnPropertyChanged(nameof(_tourManagementViewModel.NewTour));
                });
            }
        }
        
        private bool FilterBlocks(object obj)
        {
            if (obj is BlockModel block)
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                    return true;

                // Fuzzy match on block text
                if (FuzzyMatch(block.Text, SearchText) || FuzzyMatch(block.Description2, SearchText))
                    return true;

                // Find the corresponding tour for this block
                var tour = _tourManagementViewModel.Tours.FirstOrDefault(t => t.Id == block.TourID);
                if (tour != null)
                {
                    // Fuzzy match on tour properties
                    if (FuzzyMatch(tour.Name, SearchText) ||
                        FuzzyMatch(tour.Description, SearchText) ||
                        FuzzyMatch(tour.From_Location, SearchText) ||
                        FuzzyMatch(tour.To_Location, SearchText) ||
                        FuzzyMatch(tour.Transportation_Type.ToString(), SearchText) ||
                        FuzzyMatch(tour.Distance.ToString(), SearchText) ||
                        FuzzyMatch(tour.Estimated_Time.ToString(), SearchText) ||
                        FuzzyMatch(tour.Route_Information, SearchText))
                        return true;

                    // Fuzzy match on TourLogs (if present)
                    if (tour.TourLogsTable != null && tour.TourLogsTable != null)
                    {
                        foreach (var log in tour.TourLogsTable)
                        {
                            if (FuzzyMatch(log.Comment, SearchText) ||
                                FuzzyMatch(log.Date, SearchText) ||
                                FuzzyMatch(log.Difficulty, SearchText) ||
                                FuzzyMatch(log.Duration, SearchText) ||
                                FuzzyMatch(log.Rating, SearchText) ||
                                FuzzyMatch(log.Distance, SearchText) ||
                                FuzzyMatch(log.Time, SearchText))
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool FuzzyMatch(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
                return false;

            return source.ToLower().Contains(target.ToLower());
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            _propertyChangedEventHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
