using Microsoft.WindowsAPICodePack.Dialogs;
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
                        IGSubjects.Clear();
                        ALSubjects.Clear();
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

        private UpdateFrequency _updateFrequency;

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
            Properties.Settings.Default.UpdatePolicy = Convert.ToInt32(_updateFrequency);
            Properties.Settings.Default.PaperSource = PastPaperHelperCore.Source.Name.ToLower().Replace(' ', '_');
            Properties.Settings.Default.FirstRun = false;

            Properties.Settings.Default.SubjectsSubcription.Clear();
            foreach (Subject subj in PastPaperHelperCore.SubjectsLoaded)
            {
                Properties.Settings.Default.SubjectsSubcription.Add(subj.SyllabusCode);
            }

            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
        }
        #endregion

        #region LoadSubjectsCommand
        private DelegateCommand<(string, UpdateFrequency)> _loadSubjectsCommand;
        public DelegateCommand<(string, UpdateFrequency)> LoadSubjectsCommand =>
            _loadSubjectsCommand ?? (_loadSubjectsCommand = new DelegateCommand<(string, UpdateFrequency)>(ExecuteLoadSubjectsCommand));

        async void ExecuteLoadSubjectsCommand((string, UpdateFrequency) param)
        {
            UpdateFrequency updateFrequency = param.Item2;
            _updateFrequency = updateFrequency;

            string source = param.Item1;
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
        #endregion

        #region InitializeRepoCommand
        private DelegateCommand _initializeRepoCommand;
        public DelegateCommand InitializeRepoCommand =>
            _initializeRepoCommand ?? (_initializeRepoCommand = new DelegateCommand(ExecuteInitializeRepoCommand));

        void ExecuteInitializeRepoCommand()
        {
            var lst1 = from item in IGSubjects where item.IsSelected == true select item.Subject;
            var lst2 = from item in ALSubjects where item.IsSelected == true select item.Subject;
            
            foreach (Subject subj in lst1.Union(lst2))
            {
                PastPaperHelperCore.SubscribedSubjects.Add(subj);
            }
            //TODO: Download papers of each subject selected in FirstRunWindow
        }
        #endregion

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
