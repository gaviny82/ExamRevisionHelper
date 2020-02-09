﻿using Microsoft.WindowsAPICodePack.Dialogs;
using PastPaperHelper.Core.Tools;
using PastPaperHelper.Events;
using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace PastPaperHelper.ViewModels
{
    public class FirstRunWindowViewModel : BindableBase
    {
        public ObservableCollection<SubjectSelection> IGSubjects { get; set; } = new ObservableCollection<SubjectSelection>();
        public ObservableCollection<SubjectSelection> ALSubjects { get; set; } = new ObservableCollection<SubjectSelection>();

        public FirstRunWindowViewModel()
        {
            //Add notification handler
            PastPaperHelperUpdateService.UpdateServiceNotifiedEvent += (args) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (args.NotificationType == NotificationType.Initializing)
                    {
                        IsRetryEnabled = false;
                        Application.Current.Resources["IsLoading"] = Visibility.Visible;
                    }
                    if (args.NotificationType == NotificationType.Finished)
                    {
                        Application.Current.Resources["IsLoading"] = Visibility.Hidden;
                        foreach (var item in PastPaperHelperCore.SubjectsLoaded)
                        {
                            if (item.Curriculum == Curriculums.ALevel) ALSubjects.Add(new SubjectSelection(item, false));
                            else IGSubjects.Add(new SubjectSelection(item, false));
                        }
                    }
                });
            };

            //Add error handler
            PastPaperHelperUpdateService.UpdateServiceErrorEvent += (args) =>
            {
                if(args.Exception is WebException)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.Resources["IsLoading"] = Visibility.Hidden;
                    IsRetryEnabled = true;
                });
            };
        }

        private string _path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Past Papers";
        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value); }
        }

        private bool _isRetryEnabled = false;
        public bool IsRetryEnabled
        {
            get { return _isRetryEnabled; }
            set { SetProperty(ref _isRetryEnabled, value); }
        }

        #region BrowseCommand
        private DelegateCommand _browseCommand;
        public DelegateCommand BrowseCommand =>
            _browseCommand ?? (_browseCommand = new DelegateCommand(ExecuteBrowseCommand));

        void ExecuteBrowseCommand()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true })
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var path = dialog.FileName;
                    if (!path.EndsWith("Past Papers")) path += "\\Past Papers";
                    Path = path;
                }
            }
        }
        #endregion

        #region SaveCommand
        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
            _saveCommand ?? (_saveCommand = new DelegateCommand(ExecuteSaveCommand));

        void ExecuteSaveCommand()
        {
            string[] split = Path.Split('\\');
            if (!Directory.Exists(Path.Substring(0, Path.Length - split.Last().Length - 1))) return;

            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);

            Properties.Settings.Default.Path = Path;
            Properties.Settings.Default.SubjectsSubcription.Clear();
            foreach (SubjectSelection item in IGSubjects)
            {
                if (item.IsSelected == true) Properties.Settings.Default.SubjectsSubcription.Add(item.Subject.SyllabusCode);
            }
            foreach (SubjectSelection item in ALSubjects)
            {
                if (item.IsSelected == true) Properties.Settings.Default.SubjectsSubcription.Add(item.Subject.SyllabusCode);
            }
            Properties.Settings.Default.FirstRun = false;
            Properties.Settings.Default.PaperSource = "";//TODO:
            Properties.Settings.Default.UpdatePolicy = 0;//TODO:

            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
        }
        #endregion

        private DelegateCommand<string> _loadSubjectsCommand;
        public DelegateCommand<string> LoadSubjectsCommand =>
            _loadSubjectsCommand ?? (_loadSubjectsCommand = new DelegateCommand<string>(ExecuteLoadSubjectsCommand));

        async void ExecuteLoadSubjectsCommand(string source)
        {
            switch (source)
            {
                default:
                    PastPaperHelperCore.Source = new PaperSourceGCEGuide();
                    break;
                case "gce_guide":
                    PastPaperHelperCore.Source = new PaperSourceGCEGuide();
                    break;
                case "papacambridge":
                    PastPaperHelperCore.Source = new PaperSourcePapaCambridge();
                    break;
                case "cie_notes":
                    PastPaperHelperCore.Source = new PaperSourceCIENotes();
                    break;
            }
            await PastPaperHelperUpdateService.UpdateSubjectList();
        }

        #region RetryCommand
        private DelegateCommand _retryCommand;
        public DelegateCommand RetryCommand =>
            _retryCommand ?? (_retryCommand = new DelegateCommand(ExecuteRetryCommand));

        void ExecuteRetryCommand()
        {
            var task = PastPaperHelperUpdateService.UpdateSubjectList();

        }
        #endregion
    }
    public class SubjectSelection
    {
        public SubjectSelection(Subject subject, bool isSelected)
        {
            Subject = subject;
            IsSelected = isSelected;
        }

        public Subject Subject { get; set; }
        public bool? IsSelected { get; set; }
    }
}
