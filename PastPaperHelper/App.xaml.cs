using PastPaperHelper.Core.Tools;
using PastPaperHelper.Sources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace PastPaperHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public InitializationResult InitResult { get; private set; }
        public string UserDataFolderPath { get; private set; }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            UserDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\PastPaperHelper\\PastPaperHelper";
            if (!Directory.Exists(UserDataFolderPath)) Directory.CreateDirectory(UserDataFolderPath);

            StartupUri = PastPaperHelper.Properties.Settings.Default.FirstRun ? 
                new Uri("pack://application:,,,/PastPaperHelper;component/Views/OobeWindow.xaml") : 
                new Uri("pack://application:,,,/PastPaperHelper;component/Views/MainWindow.xaml");

            PaperSource source = PastPaperHelper.Properties.Settings.Default.PaperSource switch
            {
                "GCE Guide" => PaperSources.GCE_Guide,
                "PapaCambridge" => PaperSources.PapaCambridge,
                "CIE Notes" => PaperSources.CIE_Notes,
                _ => PaperSources.GCE_Guide,
            };

            //TODO: Read update policy from user preferences
            //TODO: Read subscription from user preferences
            InitResult = PastPaperHelperCore.Initialize(source, UserDataFolderPath + "\\user_data.xml", UpdatePolicy.Always, null);
        }

    }
}
