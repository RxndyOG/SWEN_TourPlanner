using System.Windows.Input;
using System.Windows.Media;

namespace Model
{
    public class BlockModel
    {
        public int TourID { get; set; }
        public string Text { get; set; }
        public string Description2 { get; set; }
        public ImageSource ImageTextBlock { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand NavigateCommandRight { get; set; }
    }
}
