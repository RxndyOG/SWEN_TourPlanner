using Microsoft.Win32;
using SWEN_TourPlanner.GUI;
using SWEN_TourPlanner.ViewModels;
using SWEN_TourPlanner.Views;
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

namespace SWEN_TourPlanner
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _output = "Hello World!";
        private string? _input;
        public ObservableCollection<BlockModel> Blocks { get; set; } = new();

        public ObservableCollection<AddTourModel> Tours { get; set; } = new ObservableCollection<AddTourModel>();

        private AddTourModel _newTour = new AddTourModel();
        public AddTourModel NewTour
        {
            get => _newTour;
            set
            {
                _newTour = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveTourCommand { get; }

        private string _newBlockText;
        public string NewBlockText
        {
            get => _newBlockText;
            set
            {
                _newBlockText = value;
                OnPropertyChanged();
            }
        }

        private Page _currentPage;

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



        private Page _currentPageRight;

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
        public ICommand NavigateCommand { get; }
        public ICommand NavigateCommandRight { get; }
        public ICommand AddBlockCommand { get; }
        public ICommand RemoveBlockCommand { get; }

        public ICommand UploadImageCommand { get; }

        //public event PropertyChangedEventHandler PropertyChanged;
        private PropertyChangedEventHandler? _propertyChangedEventHandler;
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

        public MainViewModel()
        {
            Debug.Print("ctor MainViewModel");

            NavigateCommand = new RelayCommand(Navigate);
            NavigateCommandRight = new RelayCommand(NavigateRight);

            SaveTourCommand = new RelayCommand(SaveTour);
            UploadImageCommand = new RelayCommand(UploadImageFunc);

            AddBlockCommand = new RelayCommand(_ => AddBlock());
            RemoveBlockCommand = new RelayCommand(RemoveBlock);

            #region Simpler Solution

            // Alternative: https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern#id0090030
            /*
            this.ExecuteCommand = new RelayCommand((_) => {
                Output = $"Hello {Input}!";
                Input = string.Empty;
            }, (_) => !string.IsNullOrWhiteSpace(Input));
            */

            #endregion
        }

        private void SaveTour(object parameter)
        {
            Console.WriteLine("SaveTour aufgerufen");
            if (string.IsNullOrWhiteSpace(NewTour.Name) || string.IsNullOrWhiteSpace(NewTour.From) || string.IsNullOrWhiteSpace(NewTour.To) || string.IsNullOrWhiteSpace(NewTour.Transport) || string.IsNullOrWhiteSpace(NewTour.Distance) || string.IsNullOrWhiteSpace(NewTour.Description) || string.IsNullOrWhiteSpace(NewTour.RouteInfo) || string.IsNullOrWhiteSpace(NewTour.EstimatedTime))
            {
                MessageBox.Show("Bitte füllen Sie alle Pflichtfelder aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                
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
                Text = Tour.Name,
                RemoveCommand = RemoveBlockCommand
            };

            Blocks.Add(newBlock);
            NewBlockText = "";
        }

        private void AddBlock()
        {
            if (string.IsNullOrWhiteSpace(NewBlockText))
            {
                MessageBox.Show("Bitte einen Text eingeben!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newBlock = new BlockModel
            {
                Text = NewBlockText,
                RemoveCommand = RemoveBlockCommand
            };

            Blocks.Add(newBlock);
            NewBlockText = "";
        }

        private void RemoveBlock(object block)
        {
            if (block is BlockModel blockModel && Blocks.Contains(blockModel))
            {
                

                Blocks.Remove(blockModel);
                
            }
        }

        private BitmapImage _userUploadedImage;
        public BitmapImage UserUploadedImage
        {
            get { return _userUploadedImage; }
            set
            {
                _userUploadedImage = value;
                OnPropertyChanged(nameof(UserUploadedImage));
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

      
                string fileName = Path.GetFileName(openDialog.FileName);
                string destinationPath = Path.Combine(destinationFolder, fileName);

                File.Copy(openDialog.FileName, destinationPath, true);

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
