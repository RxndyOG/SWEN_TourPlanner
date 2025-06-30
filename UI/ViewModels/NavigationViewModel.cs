using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using UI.Commands;
using UI.Views;
using log4net;
using Model;

namespace UI.ViewModels
{
    public class NavigationViewModel : INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NavigationViewModel));

        private Page _currentPageMiddle;
        private Page _currentPageRight;

        private MainViewModel _mainViewModel;

        public ICommand NavigateCommand { get; }
        public ICommand NavigateCommandRight { get; }

        public NavigationViewModel(MainViewModel mainViewModel)
        {
            NavigateCommand = new RelayCommand(Navigate);
            NavigateCommandRight = new RelayCommand(NavigateRight);

            _mainViewModel = mainViewModel;
        }

        public Page CurrentPageMiddle
        {
            get => _currentPageMiddle;
            set
            {
                if (_currentPageMiddle != value)
                {
                    _currentPageMiddle = value;
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

        private void Navigate(object parameter)
        {
            if (parameter is string pageName)
            {
                log.Info($"Navigate called with pageName: {pageName}");
                switch (pageName)
                {
                    case "Page1":
                        CurrentPageMiddle = new MainPageMiddleHome(_mainViewModel);
                        CurrentPageRight = new HomeMenuAddTour(_mainViewModel.TourManagement);
                        break;
                    case "Page2":
                        CurrentPageMiddle = new MainPageMiddleHome(_mainViewModel);
                        CurrentPageRight = new SearchMenuSearchScreen();
                        break;
                    case "Setting":
                        CurrentPageMiddle = new SettingPageMiddle();
                        CurrentPageRight = new SettingsPage1Right();
                        break;
                    default:
                        log.Warn($"Navigate: Unknown pageName '{pageName}'");
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
                        CurrentPageRight = new MainPageMiddleHome(_mainViewModel);
                        break;
                    case "Page2":
                        CurrentPageRight = new secondPageMiddleSearch();
                        break;
                    case "AddTour":
                        Debug.Print("Lade AddTour-Seite...");
                        CurrentPageRight = new HomeMenuAddTour(_mainViewModel.TourManagement);
                        break;
                    default:
                        Debug.Print("Ungültiger Seitenname");
                        break;
                }
            }
            else if (parameter is BlockModel block)
            {
                Debug.Print($"Navigiere mit Block: {block.Text}");

                // Tour anhand der ID suchen
                var tour = _mainViewModel.TourManagement.Tours.FirstOrDefault(t => t.ID == block.TourID);
                CurrentPageRight = new TourDetail(tour, _mainViewModel.TourManagement);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
