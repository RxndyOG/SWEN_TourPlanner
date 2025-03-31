using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for TourDetail.xaml
    /// </summary>
    public partial class TourDetail : Page
    {
        public MainViewModel MainVM { get; set; }
        public TourDetail(AddTourModel Tour, MainViewModel mainVM)
        {
            InitializeComponent();
            DataContext = Tour;
            MainVM = mainVM;


        }
    }
}
