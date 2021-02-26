using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml;
using ExamRevisionHelper.Core;
using ExamRevisionHelper.Core.Sources;
using ExamRevisionHelper.Views;
using Prism.Ioc;
using Prism.Unity;

namespace ExamRevisionHelper
{
    public enum InitializationResult { SuccessNoUpdate, SuccessUpdateNeeded, Error }
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public static PaperSource CurrentSource { get; private set; }
        public static ExamRevisionHelperCore CurrentInstance { get; private set; }

        public InitializationResult InitResult { get; private set; } = InitializationResult.SuccessNoUpdate;
        public ExamRevisionHelperCore CoreInstance { get; private set; }


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
            //Test: First run experience
            //setting.FirstRun = true;
            //setting.Save();
            //if (setting.FirstRun) return;

            var setting = ExamRevisionHelper.Properties.Settings.Default;

            var configDirPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PastPaperHelper\\PastPaperHelper";
            var storageDirectory = Directory.CreateDirectory(setting.Path);
            UpdateFrequency updatePolicy =  (UpdateFrequency)setting.UpdatePolicy;

            XmlDocument doc = new();
            string dataCachePath = $"{configDirPath}\\{setting.PaperSource}.xml";
            if (File.Exists(dataCachePath)) doc.Load(dataCachePath);

            var subs = setting.SubjectsSubcription;
            //Test: Invalid syllabus code.
            //subs[0] = "012345";
            string[] subsArr = new string[subs.Count];
            subs.CopyTo(subsArr, 0);

            try
            {
                CoreInstance = new ExamRevisionHelperCore(doc, storageDirectory, updatePolicy, subsArr);
                CurrentSource = (Current as App).CoreInstance.CurrentSource;
                CurrentInstance = (Current as App).CoreInstance;
                //TODO: Check update policy

                CurrentInstance.Updater.SubjectUnsubscribedEvent += (subj) =>
                {
                    var coll = setting.SubjectsSubcription;
                    if (coll.Contains(subj.SyllabusCode)) coll.Remove(subj.SyllabusCode);
                    setting.Save();
                };
            }
            catch (Exception)
            {
                //Error 1: Source not implemented
                //Error 2: Failed to load cached data, try reloading with the current source.
                //Error 3: Subscription list contains unsupported subjects for the current source.
                //             TODO: Try reloading. If error still occurred in the reload process, remove this failed subject automatically and notify the user.
                InitResult = InitializationResult.Error;
            }
        }

        public static void StartProcess(string path) => Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
    }
}
