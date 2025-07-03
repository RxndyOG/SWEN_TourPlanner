using System.Windows;
using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for HomeMenuAddTour.xaml
    /// </summary>
    public partial class HomeMenuAddTour : Page
    {
        public HomeMenuAddTour(TourManagementViewModel tourManagementvm)
        {
            InitializeComponent();
            DataContext = tourManagementvm;
        }

        private void OnShowMap(object sender, RoutedEventArgs e)
        {
            var vm = (TourManagementViewModel)DataContext;
            var from = vm.NewTour.From_Location;
            var to = vm.NewTour.To_Location;

            var mapWindow = new Window
            {
                Title = "Karte auswählen",
                Content = new MapCaptureControl(vm),
                Width = 600,
                Height = 450
            };

            mapWindow.Loaded += (s, args) =>
            {
                if (mapWindow.Content is MapCaptureControl mapControl)
                {
                    mapControl.SetRoute(from, to);
                }
            };

            mapWindow.ShowDialog();
        }
    }
}
