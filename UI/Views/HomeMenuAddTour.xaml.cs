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
    }
}
