using Microsoft.Win32;
using SWEN_TourPlanner.GUI;
using SWEN_TourPlanner.ViewModels;
using SWEN_TourPlanner.Views;
using SWEN_TourPlanner.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Prism.Events;

namespace SWEN_TourPlanner
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private Page _currentPage;
        private Page _currentPageRight;

        private PropertyChangedEventHandler? _propertyChangedEventHandler;

        public ObservableCollection<BlockModel> Blocks { get; set; } = new();
        public ObservableCollection<AddTourModel> Tours { get; set; } = new ObservableCollection<AddTourModel>();

        private AddTourModel _newTour = new AddTourModel();

        private BitmapImage _userUploadedImage;

        public ICommand SaveTourCommand { get; }
        public ICommand NavigateCommand { get; }
        public ICommand NavigateCommandRight { get; }
        public ICommand RemoveBlockCommand { get; }
        public ICommand UploadImageCommand { get; }
          
        public ICommand DeleteCommand { get; }

        public MainViewModel()
        {

            NavigateCommand = new RelayCommand(Navigate);
            NavigateCommandRight = new RelayCommand(NavigateRight);

            SaveTourCommand = new RelayCommand(SaveTour);
            UploadImageCommand = new RelayCommand(UploadImageFunc);

            RemoveBlockCommand = new RelayCommand(RemoveBlock);


            DeleteCommand = new RelayCommand(DeleteTour);
        }

        public void DeleteTour(object parameter)
        {
            
            if (parameter is int tourID)
            {
                CurrentPageRight = new DeleteWindowNothingHere();
                if (tourID >= 0 && tourID < Tours.Count)
                {
                    Tours.RemoveAt(tourID);
                }

                    Blocks.RemoveAt(tourID);

            }
        }

        public AddTourModel NewTour
        {
            get => _newTour;
            set
            {
                _newTour = value;
                OnPropertyChanged();
            }
        }

        public Page CurrentPageMiddle
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)  
                {
                    _currentPage = value;
                    OnPropertyChanged();
                }
            }
        }

        public Page CurrentPageRight
        {
            get => _currentPageRight;
            set
            {
                if (_currentPageRight != value)
                {
                    _currentPageRight = value;
                    OnPropertyChanged();
                }
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

        private void SaveTour(object parameter)
        {
            Console.WriteLine("SaveTour aufgerufen");
            if (string.IsNullOrWhiteSpace(NewTour.Name) || string.IsNullOrWhiteSpace(NewTour.From) || string.IsNullOrWhiteSpace(NewTour.To) || string.IsNullOrWhiteSpace(NewTour.Transport) || string.IsNullOrWhiteSpace(NewTour.Distance) || string.IsNullOrWhiteSpace(NewTour.Description) || string.IsNullOrWhiteSpace(NewTour.RouteInfo) || string.IsNullOrWhiteSpace(NewTour.EstimatedTime))
            {
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
                NavigateCommandRight = NavigateCommandRight
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

        private void UploadImageFunc(object parameter)
        {
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
                }


                string uniqueFileName = $"{Path.GetFileNameWithoutExtension(openDialog.FileName)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(openDialog.FileName)}";
                string destinationPath = Path.Combine(destinationFolder, uniqueFileName);
                File.Copy(openDialog.FileName, destinationPath);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    NewTour.ImagePath = destinationPath;
                    OnPropertyChanged(nameof(NewTour));
                });
            }
        }

        private void Navigate(object parameter)
        {
            if (parameter is string pageName)
            {
                switch (pageName)
                {
                    case "Page1":
                        CurrentPageMiddle = new MainPageMiddleHome(this);
                        CurrentPageRight = new HomeMenuAddTour();
                        break;
                    case "Page2":
                        CurrentPageMiddle = new secondPageMiddleSearch();
                        break;
                    case "Setting":
                        CurrentPageMiddle = new SettingPageMiddle();
                        CurrentPageRight = new SettingsPage1Right();
                        break;
                }
            }
        }

        private void NavigateRight(object parameter)
        {
            if (parameter is string pageName)
            {
                switch (pageName)
                {
                    case "Page1":
                        CurrentPageRight = new MainPageMiddleHome(this);
                        break;
                    case "Page2":
                        CurrentPageRight = new secondPageMiddleSearch();
                        break;
                    case "AddTour":
                        Debug.Print("Lade AddTour-Seite...");
                        CurrentPageRight = new HomeMenuAddTour();
                        break;
                    default:
                        Debug.Print("Ungültiger Seitenname");
                        break;
                }
            }else if (parameter is BlockModel block)
            {
                Debug.Print($"Navigiere mit Block: {block.Text}");

                var tour = Tours.FirstOrDefault(t => t.ID == block.TourID);
                CurrentPageRight = new TourDetail(tour, this);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            _propertyChangedEventHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
