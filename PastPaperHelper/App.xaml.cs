using PastPaperHelper.Sources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PastPaperHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\data")) Directory.CreateDirectory(Environment.CurrentDirectory + "\\data");
            PaperSource source = PastPaperHelper.Properties.Settings.Default.PaperSource switch
            {
                "GCE Guide" => PaperSources.GCE_Guide,
                "PapaCambridge" => PaperSources.PapaCambridge,
                "CIE Notes" => PaperSources.CIE_Notes,
                _ => PaperSources.GCE_Guide,
            };
            PaperSource.CurrentPaperSource = source;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (PastPaperHelper.Properties.Settings.Default.FirstRun)
            {
                Application.Current.StartupUri = new Uri("pack://application:,,,/PastPaperHelper;component/Views/OobeWindow.xaml");
            }
            else
            {
                Application.Current.StartupUri = new Uri("pack://application:,,,/PastPaperHelper;component/Views/MainWindow.xaml");
            }
        }
    }
}
