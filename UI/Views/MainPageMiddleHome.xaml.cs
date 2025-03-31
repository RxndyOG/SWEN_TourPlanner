using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for MainPageMiddleHome.xaml
    /// </summary>
    public partial class MainPageMiddleHome : Page
    {
        public MainPageMiddleHome(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
