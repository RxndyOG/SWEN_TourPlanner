using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SWEN_TourPlanner.ViewModels;
using SWEN_TourPlanner.Models;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SWEN_TourPlanner.Views
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
