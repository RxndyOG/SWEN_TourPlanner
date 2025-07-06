using Business.Services.Interfaces;
using Model;
using Microsoft.Win32;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using log4net;

namespace Business.Services
{
    public class ReportService : IReportService
    {
        TourLogService _tourLogService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportService));

        public ReportService(TourLogService tourLogService) 
        {
            _tourLogService = tourLogService;
        }

        public void GeneratePdfReportSingleTour(Tour tour)
        {
            log.Info("GeneratePdfReportTour called.");
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "Save PDF Report"
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                throw new Exception("Failed to open File");
            }

            string filePath = saveFileDialog.FileName;
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new Exception("Invalid file path selected");
            }

            log.Info($"Generating Report for Tour with ID: {tour.Id}");
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Tour Report";

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            var titleFont = new XFont("Arial", 10, XFontStyleEx.Regular);
            var contentFont = new XFont("Arial", 10, XFontStyleEx.Regular);

            double y = 40;
            gfx.DrawString("Tour Report", titleFont, XBrushes.Black, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
            y += 40;

            gfx.DrawString($"Name: {tour.Name}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"From: {tour.From_Location}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"To: {tour.To_Location}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Transport: {tour.Transportation_Type}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Distance: {tour.Distance}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Description: {tour.Description}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            DrawImage(gfx, tour.Route_Information, 60, 230, 450, 250);
            gfx.DrawString($"Estimated Time: {tour.Estimated_Time}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 30;

            void DrawImage(XGraphics gfx, string jpegSamplePath, int x, int y, int width, int height)
            {
                XImage image = XImage.FromFile(jpegSamplePath);
                gfx.DrawImage(image, x, y, width, height);
            }

            List<TourLog> tourLogs = _tourLogService.GetTourLogs(tour.Id);
            if (tourLogs != null && tourLogs.Count > 0)
            {
                gfx.DrawString("Tour Logs:", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                foreach (TourLog log in tourLogs)
                {
                    Console.WriteLine("Log: " + log.Id);
                    gfx.DrawString($"  - Date: {log.Logdate}, Difficulty: {log.Difficulty}, Distance: {log.Total_Distance}, Comment: {log.Comment}, Duration: {log.Total_Time}", contentFont, XBrushes.Black, new XPoint(50, y));
                    y += 20;
                    if (y > page.Height - 60)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = 40;
                    }
                }
                y += 10;
            }

            document.Save(filePath);
            log.Info($"Saved Report");
        }

        public void GeneratePdfReport(List<Tour> tours)
        {
            log.Info("GeneratePdfReport called.");

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "Save PDF Report"
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                throw new Exception("Failed to open File");
            }
                
            string filePath = saveFileDialog.FileName;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new Exception("Invalid file path selected");
            }

            PdfDocument document = new PdfDocument();
            document.Info.Title = "Tour Report";

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            var titleFont = new XFont("Arial", 10, XFontStyleEx.Regular);
            var contentFont = new XFont("Arial", 10, XFontStyleEx.Regular);

            double y = 40;
            gfx.DrawString("Tour Report", titleFont, XBrushes.Black, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
            y += 40;

            if (tours == null || tours.Count == 0)
            {
                gfx.DrawString("No tours available.", contentFont, XBrushes.Black, new XPoint(40, y));
            }
            else
            {
                // Calculate average
                double avgTourDistance = tours.Average(t => t.Distance);
                double avgTourEstimatedTime = tours.Average(t => t.Estimated_Time);

                var allLogs = tours.SelectMany(t => _tourLogService.GetTourLogs(t.Id)).ToList();
                double avgLogDistance = allLogs.Count > 0 ? allLogs.Average(l => double.TryParse((l.Total_Distance).ToString(), out var d) ? d : 0) : 0;
                double avgLogDuration = allLogs.Count > 0 ? allLogs.Average(l => double.TryParse((l.Total_Time).ToString(), out var d) ? d : 0) : 0;
                double avgLogRating = allLogs.Count > 0 ? allLogs.Average(l => double.TryParse((l.Rating).ToString(), out var d) ? d : 0) : 0;

                gfx.DrawString($"Amount of Tours: {tours.Count}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                gfx.DrawString($"Average Distance of all Tours: {avgTourDistance:F2}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                gfx.DrawString($"Average estimated time of all Tours: {avgTourEstimatedTime:F2}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                gfx.DrawString($"Amount of TourLogs: {allLogs.Count}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                gfx.DrawString($"Average Distance of all TourLogs: {avgLogDistance:F2}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                gfx.DrawString($"Average Duration of all TourLogs: {avgLogDuration:F2}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                gfx.DrawString($"Average Rating of all TourLogs: {avgLogRating:F2}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 30;

                foreach (Tour tour in tours)
                {
                    gfx.DrawString($"Name: {tour.Name}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                    gfx.DrawString($"From: {tour.From_Location}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                    gfx.DrawString($"To: {tour.To_Location}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                    gfx.DrawString($"Transport: {tour.Transportation_Type}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                    gfx.DrawString($"Distance: {tour.Distance}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                    gfx.DrawString($"Description: {tour.Description}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                    gfx.DrawString($"Image Path: {tour.Route_Information}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                    gfx.DrawString($"Estimated Time: {tour.Estimated_Time}", contentFont, XBrushes.Black, new XPoint(40, y)); y += 30;

                    List<TourLog> tourLogs = _tourLogService.GetTourLogs(tour.Id);
                    if (tourLogs != null && tourLogs.Count > 0)
                    {
                        gfx.DrawString("Tour Logs:", contentFont, XBrushes.Black, new XPoint(40, y)); y += 20;
                        foreach (TourLog log in tourLogs)
                        {
                            gfx.DrawString($"  - Date: {log.Logdate}, Difficulty: {log.Difficulty}, Distance: {log.Total_Distance}, Comment: {log.Comment}, Duration: {log.Total_Time}, Rating: {log.Rating}", contentFont, XBrushes.Black, new XPoint(50, y));
                            y += 20;
                        }
                        y += 10;
                    }
                    y += 10;
                    if (y > page.Height - 60)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = 40;
                    }
                }
            }

            document.Save(filePath);
            log.Info($"Saved Report");
        }
    }
}
