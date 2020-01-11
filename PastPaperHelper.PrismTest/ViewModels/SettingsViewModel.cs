using Microsoft.WindowsAPICodePack.Dialogs;
using PastPaperHelper.Core.Tools;
using PastPaperHelper.Models;
using PastPaperHelper.PrismTest.Views;
using PastPaperHelper.Sources;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PastPaperHelper.PrismTest.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public static ObservableCollection<Subject> IGSubjects { get; set; } = new ObservableCollection<Subject>();
        public static ObservableCollection<Subject> ALSubjects { get; set; } = new ObservableCollection<Subject>();
        public static ObservableCollection<Subject> SubjectSubscribed { get; set; } = new ObservableCollection<Subject>();

        public static void RefreshSubjectLists()
        {
            IGSubjects.Clear();
            ALSubjects.Clear();
            foreach (Subject item in PastPaperHelperCore.SubjectsLoaded)
            {
                if (item.Curriculum == Curriculums.IGCSE) IGSubjects.Add(item);
                else ALSubjects.Add(item);
            }
        }

        public static void RefreshSubscription()
        {
            SubjectSubscribed.Clear();
            foreach (KeyValuePair<Subject, PaperRepository> item in PastPaperHelperCore.Subscription)
            {
                SubjectSubscribed.Add(item.Key);
            }
        }

        public SettingsViewModel()
        {
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
                SetProperty(ref _path, value);
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
                SetProperty(ref _paperSource, value);
                if (value != null)
                {
                    PaperSource.CurrentPaperSource = value;
                    Properties.Settings.Default.PaperSource = value.Name;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private bool _autoUpdateFiles;
        public bool AutoUpdateFiles
        {
            get { return _autoUpdateFiles; }
            set
            {
                SetProperty(ref _autoUpdateFiles, value);
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
                SetProperty(ref _autoUpdateProgram, value);
                Properties.Settings.Default.AutoUpdateProgram = value;
                Properties.Settings.Default.Save();
            }
        }


        public DelegateCommand RemoveSubjectCommand { get; set; }
        private void RemoveSubjectAsync(object param)
        {
            Subject subject = (Subject)param;
            //SubscriptionManager.Unsubscribe(subject);
            SubjectSubscribed.Remove(subject);
        }

        private DelegateCommand<IList> _removeSelectedSubjectsCommand;
        public DelegateCommand<IList> RemoveSelectedSubjectsCommand =>
            _removeSelectedSubjectsCommand ?? (_removeSelectedSubjectsCommand = new DelegateCommand<IList>(ExecuteRemoveSelectedSubjectsCommand));

        void ExecuteRemoveSelectedSubjectsCommand(IList list)
        {
            while (list.Count > 0)
            {
                Subject subject = (Subject)list[0];
                SubjectSubscribed.Remove(subject);
                //TODO: SubscriptionManager.Unsubscribe(subject);
            }
        }

        private DelegateCommand<Subject> _addSubjectCommand;
        public DelegateCommand<Subject> AddSubjectCommand =>
            _addSubjectCommand ?? (_addSubjectCommand = new DelegateCommand<Subject>(ExecuteAddSubjectCommand));

        private async void ExecuteAddSubjectCommand(Subject subject)
        {
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
                    bool result = true;//TODO: await PastPaperHelperCore.Subscribe(pending[0]);
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

        private DelegateCommand _browseCommand;
        public DelegateCommand BrowseCommand =>
            _browseCommand ?? (_browseCommand = new DelegateCommand(ExecuteCommandName));

        void ExecuteCommandName()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true })
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Path = dialog.FileName;
                }
                else return;
                //TODO: Invoke Change Path Event
            }
        }

    }
}
