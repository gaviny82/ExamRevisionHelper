using PastPaperHelper.Core.Tools;
using PastPaperHelper.Models;
using PastPaperHelper.PrismTest.Commands;
using PastPaperHelper.PrismTest.Views;
using PastPaperHelper.Sources;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.IO;
using System.Windows;

namespace PastPaperHelper.PrismTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public InitializationResult InitResult { get; private set; }
        public string UserDataFolderPath { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //OOBE Test
            //PastPaperHelper.Properties.Settings.Default.FirstRun = true;
            PastPaperHelper.PrismTest.Properties.Settings.Default.FirstRun = false;
            PastPaperHelper.PrismTest.Properties.Settings.Default.Save();

            UserDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PastPaperHelper\\PastPaperHelper";
            if (!Directory.Exists(UserDataFolderPath)) Directory.CreateDirectory(UserDataFolderPath);

            PaperSource source;
            switch (PastPaperHelper.PrismTest.Properties.Settings.Default.PaperSource)
            {
                case "GCE Guide": source = PaperSources.GCE_Guide; break;
                case "PapaCambridge": source = PaperSources.PapaCambridge; break;
                case "CIE Notes": source = PaperSources.CIE_Notes; break;
                default: source = PaperSources.GCE_Guide; break;
            };

            //TODO: Read update policy from user preferences
            //TODO: Read subscription from user preferences
            InitResult = PastPaperHelperCore.Initialize(source, $"{UserDataFolderPath}\\data.xml", UpdatePolicy.Always, null);
        }

        protected override Window CreateShell()
        {
            return PastPaperHelper.PrismTest.Properties.Settings.Default.FirstRun ?
                Container.Resolve<MainWindow>() :
                Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<FilesView>("Files");
            containerRegistry.RegisterForNavigation<FilesView>("Search");
            containerRegistry.RegisterForNavigation<FilesView>("LocalStorage");
            containerRegistry.RegisterForNavigation<SettingsView>("Settings");
            containerRegistry.RegisterForNavigation<ReferenceView>("Reference");

            containerRegistry.RegisterForNavigation<SubjectDialog>("SubjectDialog");

            containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
        }
    }
}
