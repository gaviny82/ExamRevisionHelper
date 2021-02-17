using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using ExamRevisionHelper.Core;
using ExamRevisionHelper.Views;
using Prism.Ioc;
using Prism.Unity;

namespace ExamRevisionHelper
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
            return ExamRevisionHelper.Properties.Settings.Default.FirstRun ?
                (Window)Container.Resolve<FirstRunWindow>() :
                (Window)Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<FilesView>("Files");
            containerRegistry.RegisterForNavigation<SearchView>("Search");
            containerRegistry.RegisterForNavigation<DownloadView>("Download");
            containerRegistry.RegisterForNavigation<CountdownView>("Countdown");
            containerRegistry.RegisterForNavigation<MarkPaperView>("MarkPaper");
            containerRegistry.RegisterForNavigation<SettingsView>("Settings");
            containerRegistry.RegisterForNavigation<ReferenceView>("Reference");
            containerRegistry.RegisterForNavigation<PracticeView>("Practice");

            containerRegistry.RegisterForNavigation<SubjectDialog>("SubjectDialog");
        }

        private void PrismApplication_Startup(object sender, StartupEventArgs e)
        {
            UserDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PastPaperHelper\\PastPaperHelper";
            if (!Directory.Exists(UserDataFolderPath)) Directory.CreateDirectory(UserDataFolderPath);

            //Test: First run experience
            //ExamRevisionHelper.Properties.Settings.Default.FirstRun = true;
            //ExamRevisionHelper.Properties.Settings.Default.Save();
            //if (ExamRevisionHelper.Properties.Settings.Default.FirstRun) return;

            UpdateFrequency updatePolicy = (UpdateFrequency)ExamRevisionHelper.Properties.Settings.Default.UpdatePolicy;
            string dataFile = $"{UserDataFolderPath}\\{ExamRevisionHelper.Properties.Settings.Default.PaperSource}.xml";
            if (!File.Exists(dataFile)) dataFile = null;
            var subs = ExamRevisionHelper.Properties.Settings.Default.SubjectsSubcription;
            //TEST: Invalid syllabus code.
            //subs[0] = "012345";
            string[] subsArr = new string[subs.Count];
            subs.CopyTo(subsArr, 0);

            PastPaperHelperUpdateService.SubjectUnsubscribedEvent += (subj) =>
            {
                var coll = ExamRevisionHelper.Properties.Settings.Default.SubjectsSubcription;
                if (coll.Contains(subj.SyllabusCode)) coll.Remove(subj.SyllabusCode);
                ExamRevisionHelper.Properties.Settings.Default.Save();
            };
            InitResult = PastPaperHelperCore.Initialize(dataFile, ExamRevisionHelper.Properties.Settings.Default.Path, ExamRevisionHelper.Properties.Settings.Default.PaperSource, updatePolicy, subsArr);
        }

        public static void StartProcess(string path) => Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
    }
}
