using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            Properties.Settings.Default.SubjectsSubcripted.Clear();
            Properties.Settings.Default.SubjectsSubcripted.AddRange(new string[] { "0625", "9701" });

            Task.Factory.StartNew(() =>
            {
                SubjectSource[] subjects = PaperSources.GCE_Guide.GetSubjects();
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
                    App.PaperDictionary.Add(subject, papers);
                }
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
