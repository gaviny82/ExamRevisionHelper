using MaterialDesignThemes.Wpf;
using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace PastPaperHelper
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Snackbar MainSnackbar;

        public MainWindow()
        {
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTA0NzY1QDMxMzcyZTMxMmUzMEJ2UkIvNUk0T0M2OXNEYnY0cFRWUnNWUWZ0QkFyR3NVaDBlbjZuYi9NUEU9;MTA0NzY2QDMxMzcyZTMxMmUzMEVsMFlPRElYWjgyNUFYYjZBVXN2R2RHRW05QlYzYVd6S0NUL093R29PcEk9;MTA0NzY3QDMxMzcyZTMxMmUzMExYTEpvcCtUNWJ2T3NaQ01aZGJGbloxamFSN2lOaGlRM0UwUmlQcFBLSzg9");
            InitializeComponent();
            MainSnackbar = mainSnackbar;

            Task.Factory.StartNew(() =>
            {
                SubjectSource[] subjects = PaperSources.GCE_Guide.GetSubjects();
                //App.AllSubjects = subjects;
                if (!Directory.Exists(Environment.CurrentDirectory + "\\data")) Directory.CreateDirectory(Environment.CurrentDirectory + "\\data");
                PaperSource.SaveSubjectList(subjects, Environment.CurrentDirectory + "\\data\\subjects.xml", PaperSources.GCE_Guide.Name);
                App.AllSubjects = subjects;

                StringCollection subscription = Properties.Settings.Default.SubjectsSubcripted;

                foreach(string item in subscription)
                {
                    SubjectSource subject = null;
                    foreach(SubjectSource source in subjects)
                    {
                        if (source.SyllabusCode == item)
                        {
                            subject = source;
                            break;
                        }
                    }
                    if (subject == null) continue;

                    PaperItem[] papers = PaperSources.GCE_Guide.GetPapers(subject);
                    App.SubscriptionDict.Add(subject, papers);
                }
                DownloadViewModel.UpdateSubjectList();
                PaperSource.SaveSubscription(App.SubscriptionDict, Environment.CurrentDirectory + "\\data\\subscription.xml", PaperSources.GCE_Guide.Name);
            }).ContinueWith(t =>
            {
                MainSnackbar.MessageQueue.Enqueue("Data updated from " + PaperSources.GCE_Guide.Name);
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }
            MenuToggleButton.IsChecked = false;
        }
    }
}
