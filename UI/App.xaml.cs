using log4net;
using log4net.Config;
using PdfSharp.Fonts;
using PdfSharp.Fonts.OpenType;
using PdfSharp.Quality;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using UI.Converters;
using static UI.ViewModels.TourManagementViewModel;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));

        protected override void OnStartup(StartupEventArgs e)
        {
            string log4netConfigFilename = "log4net.config";
            if (!File.Exists(log4netConfigFilename))
            {
                MessageBox.Show("log4net.config not found. Logging won't work.");
            }

            // log4net.config gets copied next to exe file using Build Action

            var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(log4netConfigFilename));

            log.Debug("Debug log test");
            log.Info("Info log test");
            log.Warn("Warning log test");
            log.Error("Error log test");
            
            base.OnStartup(e);
        }
    }

}
