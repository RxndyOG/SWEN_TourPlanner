using System.Windows;
using System.Windows.Controls;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for HomeMenuAddTour.xaml
    /// </summary>
    public partial class HomeMenuAddTour : Page
    {
        public HomeMenuAddTour()
        {
            InitializeComponent();
            DataContext = Application.Current.MainWindow.DataContext;
        }
    }
}
