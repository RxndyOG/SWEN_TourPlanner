using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UI.ViewModels;

namespace UI.Views
{
    public partial class MapCaptureControl : UserControl
    {
        public event Action<string> MapImageSaved;

        private TourManagementViewModel _tourManagementViewModel;

        public MapCaptureControl(TourManagementViewModel tv)
        {
            InitializeComponent();

            _tourManagementViewModel = tv;

            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", "map.html");
            MapWebView.Source = new Uri(htmlPath);
        }

        private async void OnSaveMapImage(object sender, RoutedEventArgs e)
        {
            // Button ausblenden, damit er nicht im Screenshot ist
            BtnSaveMapImage.Visibility = Visibility.Collapsed;

            // Kurze Verzögerung, damit das UI aktualisiert wird
            await System.Threading.Tasks.Task.Delay(200);

            // Screenshot vom Kartenbereich (WebView2)
            var renderTarget = new RenderTargetBitmap(
                (int)MapWebView.ActualWidth,
                (int)MapWebView.ActualHeight,
                96, 96, System.Windows.Media.PixelFormats.Pbgra32);
            renderTarget.Render(MapWebView);

            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", "Images");
            Directory.CreateDirectory(imagesFolder);
            string filePath = Path.Combine(imagesFolder, $"Map_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));
                encoder.Save(fileStream);
                
            }

            if (DataContext is MainViewModel vm && _tourManagementViewModel.NewTour != null)
            {
                _tourManagementViewModel.NewTour.ImagePath = new Uri(filePath).AbsoluteUri;
            }


            // Button wieder einblenden
            BtnSaveMapImage.Visibility = Visibility.Visible;

            MapImageSaved?.Invoke(filePath);
            MessageBox.Show("Kartenbild gespeichert: " + filePath);

            
        }
    }
}
