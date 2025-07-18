﻿using System;
using System.Diagnostics;
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

        public async Task SetRoute(string from, string to)
        {
            if (MapWebView.CoreWebView2 != null)
            {
                Debug.WriteLine("WebView2 already initialized — executing script directly");
                string script = $"setRoute({System.Text.Json.JsonSerializer.Serialize(from)}, {System.Text.Json.JsonSerializer.Serialize(to)})";
                Debug.WriteLine($"Executing script: {script}");
                await MapWebView.ExecuteScriptAsync(script);
            }
            else
            {
                Debug.WriteLine("WebView2 not ready yet — attaching NavigationCompleted");
                MapWebView.NavigationCompleted += async (s, e) =>
                {
                    Debug.WriteLine("WebView finished loading (NavigationCompleted)");
                    string script = $" setRoute({System.Text.Json.JsonSerializer.Serialize(from)}, {System.Text.Json.JsonSerializer.Serialize(to)})";
                    Debug.WriteLine($"Executing script: {script}");
                    await MapWebView.ExecuteScriptAsync(script);
                };
            }

        }

        private async void OnSaveMapImage(object sender, RoutedEventArgs e)
        {
            BtnSaveMapImage.Visibility = Visibility.Collapsed;
            await System.Threading.Tasks.Task.Delay(200);

            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", "Images");
            Directory.CreateDirectory(imagesFolder);
            string filePath = Path.Combine(imagesFolder, $"Map_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            // WebView2 Screenshot
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                // Achtung: WebView2 muss geladen sein!
                await MapWebView.EnsureCoreWebView2Async();
                Debug.WriteLine("WebView2 is ready.");
                
                await MapWebView.CoreWebView2.CapturePreviewAsync(
                    Microsoft.Web.WebView2.Core.CoreWebView2CapturePreviewImageFormat.Png, stream);

            }

            if (_tourManagementViewModel.NewTour != null)
            {
                _tourManagementViewModel.NewTour.Image_Path = filePath;
            }

            BtnSaveMapImage.Visibility = Visibility.Visible;
            MapImageSaved?.Invoke(filePath);

            MessageBox.Show("Kartenbild gespeichert: " + filePath);
        }
    }
}
