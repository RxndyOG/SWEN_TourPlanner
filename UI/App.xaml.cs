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

namespace SWEN_TourPlanner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            
        }
    }

}
