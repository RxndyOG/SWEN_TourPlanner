using System.Windows.Controls;
using Model;

namespace UI.Views
{
    /// <summary>
    /// Interaction logic for TourLogsTable.xaml
    /// </summary>
    public partial class TourLogsTable : UserControl
    {
        public TourLogsTable()
        {
            InitializeComponent();
            DataContext = new TourLogs();
        }
    }
}
