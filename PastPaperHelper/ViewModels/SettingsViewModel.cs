using Microsoft.WindowsAPICodePack.Dialogs;
using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using PastPaperHelper.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PastPaperHelper.ViewModels
{
    class SettingsViewModel : NotificationObject
    {
        public static ObservableCollection<Subject> IGSubjects { get; set; } = new ObservableCollection<Subject>();
        public static ObservableCollection<Subject> ALSubjects { get; set; } = new ObservableCollection<Subject>();
        public static ObservableCollection<Subject> SubjectSubscribed { get; set; } = new ObservableCollection<Subject>();

        public static void RefreshSubjectLists()
        {
            IGSubjects.Clear();
            ALSubjects.Clear();
            foreach (Subject item in SubscriptionManager.AllSubjects)
            {
                if (item.Curriculum == Curriculums.IGCSE) IGSubjects.Add(item);
                else ALSubjects.Add(item);
            }
        }

        public static void RefreshSubscription()
        {
            SubjectSubscribed.Clear();
            foreach (KeyValuePair<Subject, PaperRepository> item in SubscriptionManager.Subscription)
            {
                SubjectSubscribed.Add(item.Key);
            }
        }


        public SettingsViewModel()
        {
            RemoveSelectedSubjectsCommand = new DelegateCommand(RemoveSelectedSubjects);
            RemoveSubjectCommand = new DelegateCommand(RemoveSubjectAsync);
            AddSubjectCommand = new DelegateCommand(AddSubject);
            BrowseCommand = new DelegateCommand(Browse);

            Path = Properties.Settings.Default.Path;
            PaperSource = PaperSource.CurrentPaperSource;

            AutoUpdateFiles = Properties.Settings.Default.AutoUpdateFiles;
            AutoUpdateProgram = Properties.Settings.Default.AutoUpdateProgram;
        }


        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChangedEvent("Path");
                if (Directory.Exists(value))
                {
                    Properties.Settings.Default.Path = value;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private PaperSource _paperSource;
        public PaperSource PaperSource
        {
            get { return _paperSource; }
            set
            {
                _paperSource = value;
                RaisePropertyChangedEvent("PaperSource");
                if (value == null) return;
                PaperSource.CurrentPaperSource = value;
                Properties.Settings.Default.PaperSource = value.Name;
                Properties.Settings.Default.Save();
            }
        }

        private bool _autoUpdateFiles;
        public bool AutoUpdateFiles
        {
            get { return _autoUpdateFiles; }
            set
            {
                _autoUpdateFiles = value;
                RaisePropertyChangedEvent("AutoUpdateFiles");
                Properties.Settings.Default.AutoUpdateFiles = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool _autoUpdateProgram;
        public bool AutoUpdateProgram
        {
            get { return _autoUpdateProgram; }
            set
            {
                _autoUpdateProgram = value;
                RaisePropertyChangedEvent("AutoUpdateProgram");
                Properties.Settings.Default.AutoUpdateProgram = value;
                Properties.Settings.Default.Save();
            }
        }


        public DelegateCommand RemoveSubjectCommand { get; set; }
        private void RemoveSubjectAsync(object param)
        {
            Subject subject = (Subject)param;
            SubscriptionManager.Unsubscribe(subject);
            SubjectSubscribed.Remove(subject);
        }

        public DelegateCommand RemoveSelectedSubjectsCommand { get; set; }
        private void RemoveSelectedSubjects(object param)
        {
            IList list = (IList)param;
            while (list.Count > 0)
            {
                Subject subject = (Subject)list[0];
                SubjectSubscribed.Remove(subject);
                SubscriptionManager.Unsubscribe(subject);
            }
        }

        public DelegateCommand AddSubjectCommand { get; set; }
        private async void AddSubject(object param)
        {
            Subject subject = (Subject)param;
            pending.Add(subject);
            await CheckSubscription();
        }

        private bool isLoading = false;
        private List<Subject> pending = new List<Subject>();
        public async Task CheckSubscription()
        {
            if (isLoading) return;
            isLoading = true;
            Application.Current.MainWindow.Resources["IsLoading"] = Visibility.Visible;

            while (pending.Count != 0)
            {
                try
                {
                    bool result = await SubscriptionManager.Subscribe(pending[0]);
                    if (result) SubjectSubscribed.Add(pending[0]);
                    pending.RemoveAt(0);
                }
                catch (Exception)
                {
                    await Task.Factory.StartNew(() => MainWindow.MainSnackbar.MessageQueue.Enqueue("Failed to fetch data from " + PaperSource.CurrentPaperSource.Name + ", please check your Internet connection.\nYour subjects will be synced when connected to Internet"), new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
                    isLoading = false;
                    Application.Current.MainWindow.Resources["IsLoading"] = Visibility.Hidden;
                    return;
                }
            }
            isLoading = false;
            Application.Current.MainWindow.Resources["IsLoading"] = Visibility.Hidden;
        }

        public DelegateCommand BrowseCommand { get; set; }
        private void Browse(object param)
        {
            using(CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true })
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Path = dialog.FileName;
                }
                else return;
                SearchViewModel model = ((Application.Current.MainWindow.DataContext as MainWindowViewModel).ListItems[1].Content as SearchView).DataContext as SearchViewModel;
                model.SearchPath = Path;//TODO: Use binding instead
            }
        }
    }
}
