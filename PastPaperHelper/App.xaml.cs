using PastPaperHelper.Sources;
using System;
using System.Windows;

namespace PastPaperHelper
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            PaperSource source = null;
            switch (PastPaperHelper.Properties.Settings.Default.PaperSource)
            {
                default: source = PaperSources.GCE_Guide; break;
                case "GCE Guide": source = PaperSources.GCE_Guide; break;
                case "PapaCambridge": source = PaperSources.PapaCambridge; break;
                case "CIE Notes": source = PaperSources.CIE_Notes; break;
            }
            SubscriptionManager.CurrentPaperSource = source;

            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTA0NzY1QDMxMzcyZTMxMmUzMEJ2UkIvNUk0T0M2OXNEYnY0cFRWUnNWUWZ0QkFyR3NVaDBlbjZuYi9NUEU9;MTA0NzY2QDMxMzcyZTMxMmUzMEVsMFlPRElYWjgyNUFYYjZBVXN2R2RHRW05QlYzYVd6S0NUL093R29PcEk9;MTA0NzY3QDMxMzcyZTMxMmUzMExYTEpvcCtUNWJ2T3NaQ01aZGJGbloxamFSN2lOaGlRM0UwUmlQcFBLSzg9");
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (PastPaperHelper.Properties.Settings.Default.FirstRun)
            {
                Application.Current.StartupUri = new Uri("pack://application:,,,/PastPaperHelper;component/Views/OobeWindow.xaml");
            }
            else
            {
                Application.Current.StartupUri = new Uri("pack://application:,,,/PastPaperHelper;component/MainWindow.xaml");
            }
        }
    }
}
