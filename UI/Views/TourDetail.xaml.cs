using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for TourDetail.xaml
    /// </summary>
    public partial class TourDetail : Page
    {
        public AddTourModel Tour { get; set; }
        public TourManagementViewModel TourManagement { get; set; }

        public TourDetail(AddTourModel tour, TourManagementViewModel tourManagement)
        {
            InitializeComponent();
            Tour = tour;
            TourManagement = tourManagement;

            // Setze DataContext auf ein passendes Objekt, z.B. ein ViewModel, das Tour und TourManagement bereitstellt
            DataContext = Tour;
        }
    }
}
