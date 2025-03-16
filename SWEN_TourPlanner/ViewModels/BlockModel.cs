using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SWEN_TourPlanner.GUI
{
    public class BlockModel
    {
        public int TourID { get; set; }
        public string Text { get; set; }
        public ImageSource Image { get; set; }
        public ICommand RemoveCommand { get; set; }
    }
}
