using Microsoft.Win32;
using SWEN_TourPlanner.GUI;
using SWEN_TourPlanner.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
        public ObservableCollection<TableRow> TableData { get; set; }
        public ObservableCollection<BlockModel> Blocks { get; set; } = new();

        public string Input
        {
            get => _input;
            set
            {
                _input = value;
                OnPropertyChanged();
            }
        }
        public string Output
        {
            get => _output;
            set
            {
                _output = value;
                OnPropertyChanged();
            }
        }

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

        public ICommand UploadImage { get; }

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

        public class TableRow
        {
            public string Column1 { get; set; }
            public string Column2 { get; set; }
            public string Column3 { get; set; }
        }

        

        public MainViewModel()
        {
            Debug.Print("ctor MainViewModel");

            NavigateCommand = new RelayCommand(Navigate);
            NavigateCommandRight = new RelayCommand(NavigateRight);

            UploadImage = new RelayCommand(UploadImageFunc);

            AddBlockCommand = new RelayCommand(_ => AddBlock());
            RemoveBlockCommand = new RelayCommand(RemoveBlock);

            TableData = new ObservableCollection<TableRow>
            {
                new TableRow { Column1 = "Wert 1", Column2 = "Wert A", Column3 = "Wert X" },
                new TableRow { Column1 = "Wert 2", Column2 = "Wert B", Column3 = "Wert Y" }
                
            };
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
            NewBlockText = "";  // Eingabefeld zurücksetzen
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
                OnPropertyChanged(nameof(UserUploadedImage)); // Ensure property change notification
            }
        }

        private void UploadImageFunc(object parameter)
        {
            OpenFileDialog openDialogue = new OpenFileDialog();
            openDialogue.Filter = "Image files*.bmp;*.jpg;*png";
            openDialogue.FilterIndex = 1;
            if (openDialogue.ShowDialog() == true)
            {
                UserUploadedImage = new BitmapImage(new Uri(openDialogue.FileName));
            }
        }

        private void Navigate(object parameter)
        {
            if (parameter is string pageName)
            {
                switch (pageName)
                {
                    case "Page1":
                        CurrentPageMiddle = new MainPageMiddleHome();
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
                        CurrentPageRight = new MainPageMiddleHome();
                        break;
                    case "Page2":
                        CurrentPageRight = new secondPageMiddleSearch();
                        break;
                    case "AddTour":
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
