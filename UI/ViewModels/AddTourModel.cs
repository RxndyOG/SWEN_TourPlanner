using UI.Commands;
using UI.Exceptions;
using Business.Services;
using DataAccess.Database;
using DataAccess.Repositories;
using Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;

namespace UI.ViewModels
{
    public class AddTourModel : INotifyPropertyChanged
    {
        private Tour _tour;
        private TourLogs _tourLogs;
        private string _image_Path;
        private TourLogService _tourLogService;


        public ICommand AddTourLogCommand { get; }
        public ICommand RemoveTourLogCommand { get; }
        public ICommand ModifyTourLogsCommand { get; }

        public AddTourModel()
        {
            _tour = new Tour();
            _tourLogs = new TourLogs();

            _tourLogService = new TourLogService(new TourLogRepository(new Database()));

            ModifyTourLogsCommand = new RelayCommand(ModifyTourLogs);
            AddTourLogCommand = new RelayCommand(SaveTourLog);
            RemoveTourLogCommand = new RelayCommand(RemoveTourLog);
        }

        // Model-Kapselung
        public Tour Tour
        {
            get => _tour;
            set
            {
                _tour = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Id));
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(From_Location));
                OnPropertyChanged(nameof(To_Location));
                OnPropertyChanged(nameof(Transportation_Type));
                OnPropertyChanged(nameof(Distance));
                OnPropertyChanged(nameof(Estimated_Time));
                OnPropertyChanged(nameof(Route_Information));
            }
        }

        public TourLogs TourLogs
        {
            get => _tourLogs;
            set
            {
                _tourLogs = value;
                OnPropertyChanged();
            }
        }

        // Convenience-Properties für Data Binding
        public int Id
        {
            get => _tour.Id;
            set { _tour.Id = value; OnPropertyChanged(); }
        }
        public string Name
        {
            get => _tour.Name;
            set { _tour.Name = value; OnPropertyChanged(); }
        }
        public string Description
        {
            get => _tour.Description;
            set { _tour.Description = value; OnPropertyChanged(); }
        }
        public string From_Location
        {
            get => _tour.From_Location;
            set { _tour.From_Location = value; OnPropertyChanged(); }
        }
        public string To_Location
        {
            get => _tour.To_Location;
            set { _tour.To_Location = value; OnPropertyChanged(); }
        }
        public string Transportation_Type
        {
            get => _tour.Transportation_Type;
            set { _tour.Transportation_Type = value; OnPropertyChanged(); }
        }

        public int Distance
        {
            get => _tour.Distance;
            set { _tour.Distance = value; OnPropertyChanged(); }
        }
        public int Estimated_Time
        {
            get => _tour.Estimated_Time;
            set { _tour.Estimated_Time = value; OnPropertyChanged(); }
        }
        public string Route_Information
        {
            get => _tour.Route_Information;
            set { _tour.Route_Information = value; OnPropertyChanged(); }
        }

        // Optional: ImagePath für UI
        public string Image_Path
        {
            get => _image_Path;
            set { _image_Path = value; OnPropertyChanged(); }
        }

        // Log-Handling
        public ObservableCollection<TourLogs.TourLog> TourLogsTable => _tourLogs.TourLogsTable;

        private int _logIdToRemove;
        public int LogIdToRemove
        {
            get => _logIdToRemove;
            set { _logIdToRemove = value; OnPropertyChanged(); }
        }


        private void SaveTourLog(object parameter)
        {

            try
            {

                // Beispiel: Einfache Validierung, kann angepasst werden
                if (string.IsNullOrWhiteSpace(TourLogs.Date) ||
                    string.IsNullOrWhiteSpace(TourLogs.Comment))
                {
                    Exception e = new MissingRequiredFieldException();
                    MessageBox.Show(e.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning); // TODO eine ebene runter
                    throw e;
                }

            }catch (MissingRequiredFieldException e)
            {
                MessageBox.Show(e.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Abbrechen, wenn Validierung fehlschlägt
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ein unerwarteter Fehler ist aufgetreten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Abbrechen, wenn ein unerwarteter Fehler auftritt
            }

            var newLog = new TourLogs.TourLog
            {
                IDTourLogs = TourLogs.TourLogsTable.Count + 1,
                Date = TourLogs.Date,
                Time = TourLogs.Time,
                Difficulty = TourLogs.Difficulty,
                Comment = TourLogs.Comment,
                Rating = TourLogs.Rating,
                Distance = TourLogs.Distance,
                Duration = TourLogs.Duration,
            };

            TourLogs.TourLogsTable.Add(newLog);

            var dbLog = new Model.TourLog
            {
                Tour_Id = this.Id,
                Logdate = DateTime.TryParse(newLog.Date + " " + newLog.Time, out var dt) ? dt : DateTime.Now,
                Comment = newLog.Comment,
                Difficulty = newLog.Difficulty,
                Total_Distance = int.TryParse(newLog.Distance, out var dist) ? dist : 0,
                Total_Time = int.TryParse(newLog.Duration, out var dur) ? dur : 0,
                Rating = int.TryParse(newLog.Rating, out var rat) ? rat : 1
            };

            _tourLogService.AddTourLog(dbLog);

            // Felder zurücksetzen
            TourLogs.Date = string.Empty;
            TourLogs.Time = string.Empty;
            TourLogs.Difficulty = string.Empty;
            TourLogs.Comment = string.Empty;
            TourLogs.Rating = string.Empty;
            TourLogs.Distance = string.Empty;
            TourLogs.Duration = string.Empty;
            OnPropertyChanged(nameof(TourLogs));
        }

        private void ModifyTourLogs(object parameter)
        {
            foreach (var log in TourLogsTable)
            {
                try
                {
                    // Hole die Tour Logs aus der Datenbank
                    var dbLogs = _tourLogService.GetTourLogs(this.Id);
                    var dbLog = dbLogs.FirstOrDefault(l => l.Id == log.IDTourLogs);

                    if (dbLog != null)
                    {
                        // Aktualisiere die Werte im Datenbank-Log
                        dbLog.Logdate = DateTime.TryParse($"{log.Date} {log.Time}", out var dt) ? dt : dbLog.Logdate;
                        dbLog.Comment = log.Comment;
                        dbLog.Difficulty = log.Difficulty;
                        dbLog.Total_Distance = int.TryParse(log.Distance, out var dist) ? dist : dbLog.Total_Distance;
                        dbLog.Total_Time = int.TryParse(log.Duration, out var dur) ? dur : dbLog.Total_Time;
                        dbLog.Rating = int.TryParse(log.Rating, out var rat) ? rat : dbLog.Rating;

                        // Speichere die Änderungen in der Datenbank
                        _tourLogService.UpdateTourLog(dbLog);
                        Debug.WriteLine($"TourLog mit ID {log.IDTourLogs} erfolgreich aktualisiert.");
                    }
                    else
                    {
                        Debug.WriteLine($"Kein TourLog mit ID {log.IDTourLogs} gefunden.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Speichern des Logs mit ID {log.IDTourLogs}: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Erfolgsmeldung
            MessageBox.Show("Alle Änderungen an den TourLogs wurden erfolgreich gespeichert!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RemoveTourLog(object parameter)
        {
            var log = TourLogs.TourLogsTable.FirstOrDefault(t => t.IDTourLogs == LogIdToRemove);
            if (log != null)
            {
                // Versuche, das Log auch aus der Datenbank zu löschen (falls vorhanden)
                // Suche das passende DB-Log über TourId und evtl. weitere Felder
                var dbLogs = _tourLogService.GetTourLogs(this.Id);
                var dbLog = dbLogs.FirstOrDefault(l =>
                    l.Comment == log.Comment &&
                    l.Tour_Id == this.Id &&
                    l.Logdate.ToShortDateString() == log.Date &&
                    l.Logdate.ToShortTimeString() == log.Time
                );
                if (dbLog != null)
                {
                    _tourLogService.DeleteTourLog(dbLog.Id);
                }

                TourLogs.TourLogsTable.Remove(log);
            }
            else
            {
                Exception e = new TourLogNotFoundException(LogIdToRemove);
                MessageBox.Show(e.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning); // TODO eine ebene runter
                return;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}