using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Model;
using UI.Commands;

namespace UI.ViewModels
{
    public class AddTourModel : INotifyPropertyChanged
    {
        private int _id;
        private int _idTour;
        private string _name;
        private string _from;
        private string _to;
        private string _description;
        private string _transport;
        private string _distance;
        private string _estimatedTime;
        private string _routeInfo;
        private string _imagePath;
        public ObservableCollection<TourLogs> Tours { get; set; } = new ObservableCollection<TourLogs>();

        private TourLogs _newTourLog = new TourLogs();

        public ICommand AddTourLogCommand { get; }
        public ICommand RemoveTourCommand { get; }
        public AddTourModel()
        {
            AddTourLogCommand = new RelayCommand(SaveTourLog);
            RemoveTourCommand = new RelayCommand(RemoveTourLog);
        }

        
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

        private void SaveTourLog(object parameter)
        {
            Console.WriteLine("SaveTour aufgerufen");

            if (TourLog == null)
            {
                MessageBox.Show("Tour Log is null!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TourLog.Date) || string.IsNullOrWhiteSpace(TourLog.Time) ||
                string.IsNullOrWhiteSpace(TourLog.Difficulty) || string.IsNullOrWhiteSpace(TourLog.Duration) ||
                string.IsNullOrWhiteSpace(TourLog.Distance) || string.IsNullOrWhiteSpace(TourLog.Rating) ||
                string.IsNullOrWhiteSpace(TourLog.Comment))
            {
                MessageBox.Show("Required Input is Missing", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TourLogs.TourLog newLog = new TourLogs.TourLog
            {
                IDTourLogs = TourLog.TourLogsTable.Count + 1,
                Date = TourLog.Date,
                Time = TourLog.Time,
                Difficulty = TourLog.Difficulty,
                Comment = TourLog.Comment,
                Rating = TourLog.Rating,
                Distance = TourLog.Distance,
                Duration = TourLog.Duration,
            };

            TourLog.TourLogsTable.Add(newLog);

            Console.WriteLine($"Neues TourLog hinzugefügt mit ID: {newLog.IDTourLogs}");

            TourLog.Date = string.Empty;
            TourLog.Time = string.Empty;
            TourLog.Difficulty = string.Empty;
            TourLog.Comment = string.Empty;
            TourLog.Rating = string.Empty;
            TourLog.Distance = string.Empty;
            TourLog.Duration = string.Empty;
            OnPropertyChanged(nameof(TourLog));
        }

        private void RemoveTourLog(object parameter)
        {
            if (IDTourLogsTest <= 0)
            {
                MessageBox.Show("Bitte eine gültige ID eingeben!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var tourToRemove = TourLog.TourLogsTable.FirstOrDefault(t => t.IDTourLogs == IDTourLogsTest);
            if (tourToRemove != null)
            {
                TourLog.TourLogsTable.Remove(tourToRemove);
                Console.WriteLine($"TourLog mit ID {IDTourLogsTest} wurde entfernt.");
            }
            else
            {
                MessageBox.Show($"Kein TourLog mit ID {IDTourLogsTest} gefunden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public TourLogs TourLog
        {
            get => _newTourLog;
            set
            {
                _newTourLog = value;
                OnPropertyChangedLog();
            }
        }

        public int IDTourLogsTest
        {
            get => _idTour;
            set { _idTour = value; OnPropertyChanged(nameof(IDTourLogsTest)); }
        }
        public int ID
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(ID)); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public string From
        {
            get => _from;
            set { _from = value; OnPropertyChanged(nameof(From)); }
        }

        public string To
        {
            get => _to;
            set { _to = value; OnPropertyChanged(nameof(To)); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        public string Transport
        {
            get => _transport;
            set { _transport = value; OnPropertyChanged(nameof(Transport)); }
        }

        public string Distance
        {
            get => _distance;
            set { _distance = value; OnPropertyChanged(nameof(Distance)); }
        }

        public string EstimatedTime
        {
            get => _estimatedTime;
            set { _estimatedTime = value; OnPropertyChanged(nameof(EstimatedTime)); }
        }

        public string RouteInfo
        {
            get => _routeInfo;
            set { _routeInfo = value; OnPropertyChanged(nameof(RouteInfo)); }
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private PropertyChangedEventHandler? _propertyChangedEventHandler;
        protected virtual void OnPropertyChangedLog([CallerMemberName] string? propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            _propertyChangedEventHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
