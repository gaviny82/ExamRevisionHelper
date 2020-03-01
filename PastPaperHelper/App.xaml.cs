using PastPaperHelper.Core.Tools;
using PastPaperHelper.Models;
using PastPaperHelper.Views;
using PastPaperHelper.Sources;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.IO;
using System.Windows;
using PastPaperHelper.ViewModels;

namespace PastPaperHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public InitializationResult InitResult { get; private set; }
        public string UserDataFolderPath { get; private set; }

        protected override Window CreateShell()
        {
            return PastPaperHelper.Properties.Settings.Default.FirstRun ?
                (Window)Container.Resolve<FirstRunWindow>() :
                (Window)Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<FilesView>("Files");
            //containerRegistry.RegisterForNavigation<FilesView>("Search");
            containerRegistry.RegisterForNavigation<DownloadView>("Download");
            containerRegistry.RegisterForNavigation<SettingsView>("Settings");
            containerRegistry.RegisterForNavigation<ReferenceView>("Reference");

            containerRegistry.RegisterForNavigation<SubjectDialog>("SubjectDialog");
        }

        private void PrismApplication_Startup(object sender, StartupEventArgs e)
        {
            UserDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PastPaperHelper\\PastPaperHelper";
            if (!Directory.Exists(UserDataFolderPath)) Directory.CreateDirectory(UserDataFolderPath);

            //Test: First run experience
            //PastPaperHelper.Properties.Settings.Default.FirstRun = true;
            //PastPaperHelper.Properties.Settings.Default.Save();
            //if (PastPaperHelper.Properties.Settings.Default.FirstRun) return;

            UpdateFrequency updatePolicy = (UpdateFrequency)PastPaperHelper.Properties.Settings.Default.UpdatePolicy;
            string dataFile = $"{UserDataFolderPath}\\{PastPaperHelper.Properties.Settings.Default.PaperSource}.xml";
            if (!File.Exists(dataFile)) dataFile = null;
            var subs = PastPaperHelper.Properties.Settings.Default.SubjectsSubcription;
            //TEST: Invalid syllabus code.
            //subs[0] = "012345";
            string[] subsArr = new string[subs.Count];
            subs.CopyTo(subsArr, 0);

            PastPaperHelperUpdateService.SubjectUnsubscribedEvent += (subj) =>
            {
                var coll = PastPaperHelper.Properties.Settings.Default.SubjectsSubcription;
                if (coll.Contains(subj.SyllabusCode)) coll.Remove(subj.SyllabusCode);
                PastPaperHelper.Properties.Settings.Default.Save();
            };
            InitResult = PastPaperHelperCore.Initialize(dataFile, PastPaperHelper.Properties.Settings.Default.Path, PastPaperHelper.Properties.Settings.Default.PaperSource, updatePolicy, subsArr);
        }
    }
}
