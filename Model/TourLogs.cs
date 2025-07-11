﻿using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Model
{
    public class TourLogs : INotifyPropertyChanged
    {
        private int _id;
        private string _date;
        private string _comment;
        private string _difficulty;
        private string _duration;
        private string _rating;
        private string _time;
        private string _distance;
        private string _routeInfo;
        public ObservableCollection<TourLog> TourLogsTable { get; set; }
        public class TourLog : INotifyPropertyChanged
        {
            private int _idTourLogs;
            private string _date;
            private string _time;
            private string _comment;
            private string _difficulty;
            private string _distance;
            private string _duration;
            private string _rating;

            public int IDTourLogs
            {
                get => _idTourLogs;
                set { _idTourLogs = value; OnPropertyChanged(nameof(IDTourLogs)); }
            }
            public string Date
            {
                get => _date;
                set { _date = value; OnPropertyChanged(nameof(Date)); }
            }
            public string Time
            {
                get => _time;
                set { _time = value; OnPropertyChanged(nameof(Time)); }
            }
            public string Comment
            {
                get => _comment;
                set { _comment = value; OnPropertyChanged(nameof(Comment)); }
            }
            public string Difficulty
            {
                get => _difficulty;
                set { _difficulty = value; OnPropertyChanged(nameof(Difficulty)); }
            }
            public string Distance
            {
                get => _distance;
                set { _distance = value; OnPropertyChanged(nameof(Distance)); }
            }
            public string Duration
            {
                get => _duration;
                set { _duration = value; OnPropertyChanged(nameof(Duration)); }
            }
            public string Rating
            {
                get => _rating;
                set { _rating = value; OnPropertyChanged(nameof(Rating)); }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public TourLogs()
        {
            TourLogsTable = new ObservableCollection<TourLog>();

        }

        public int IDTourLogs
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(IDTourLogs)); }
        }

        public string Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(nameof(Date)); }
        }

        public string Comment
        {
            get => _comment;
            set { _comment = value; OnPropertyChanged(nameof(Comment)); }
        }

        public string Difficulty
        {
            get => _difficulty;
            set { _difficulty = value; OnPropertyChanged(nameof(Difficulty)); }
        }

        public string Duration
        {
            get => _duration;
            set { _duration = value; OnPropertyChanged(nameof(Duration)); }
        }

        public string Rating
        {
            get => _rating;
            set { _rating = value; OnPropertyChanged(nameof(Rating)); }
        }

        public string Distance
        {
            get => _distance;
            set { _distance = value; OnPropertyChanged(nameof(Distance)); }
        }

        public string Time
        {
            get => _time;
            set { _time = value; OnPropertyChanged(nameof(Time)); }
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
    }
}
