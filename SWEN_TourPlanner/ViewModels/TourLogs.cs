using SWEN_TourPlanner.GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_TourPlanner.ViewModels
{
    class TourLogs  : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private string _from;
        private string _to;
        private string _description;
        private string _transport;
        private string _distance;
        private string _estimatedTime;
        private string _routeInfo;
        private string _imagePath;

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

        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(nameof(ImagePath)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    
    }   
}
