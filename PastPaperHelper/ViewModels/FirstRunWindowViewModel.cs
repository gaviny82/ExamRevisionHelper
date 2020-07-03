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
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

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
                        IsProceedAllowed = false;
                        IsRevertAllowed = false;
                        Application.Current.Resources["IsLoading"] = Visibility.Visible;
                    }
                    if (args.NotificationType == NotificationType.Finished)
                    {
                        IsRevertAllowed = true;
                        IsProceedAllowed = true;
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
                    IsRevertAllowed = true;
                    IsProceedAllowed = false;
                });
            };
        }

        private string _path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Past Papers";
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

        private bool _isRetryEnabled2 = false;
        public bool IsRetryEnabled2
        {
            get { return _isRetryEnabled2; }
            set { SetProperty(ref _isRetryEnabled2, value); }
        }

        private string _updateMessage = "Updating subjects...";
        public string UpdateMessage
        {
            get { return _updateMessage; }
            set { SetProperty(ref _updateMessage, value); }
        }

        private string _updateTitle = "Almost done";
        public string UpdateTitle
        {
            get { return _updateTitle; }
            set { SetProperty(ref _updateTitle, value); }
        }

        private int _updateCount;
        public int UpdateCount
        {
            get { return _updateCount; }
            set { SetProperty(ref _updateCount, value); }
        }

        private int _totalSubscribed;
        public int TotalSubscribed
        {
            get { return _totalSubscribed; }
            set { SetProperty(ref _totalSubscribed, value); }
        }

        private bool _isRevertAllowed = true;
        public bool IsRevertAllowed
        {
            get { return _isRevertAllowed; }
            set { SetProperty(ref _isRevertAllowed, value); }
        }

        private bool _isProceedAllowed = true;
        public bool IsProceedAllowed
        {
            get { return _isProceedAllowed; }
            set { SetProperty(ref _isProceedAllowed, value); }
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

        async void ExecuteSaveCommand()
        {
            string[] split = Path.Split('\\');
            if (!Directory.Exists(Path.Substring(0, Path.Length - split.Last().Length - 1))) return;
            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);

            Properties.Settings.Default.Path = Path;
            string source = PastPaperHelperCore.Source.Name;
            Properties.Settings.Default.PaperSource = source;

            Properties.Settings.Default.SubjectsSubcription.Clear();
            foreach (Subject subj in PastPaperHelperCore.SubscribedSubjects)
            {
                Properties.Settings.Default.SubjectsSubcription.Add(subj.SyllabusCode);
            }

            await Task.Run(() =>
            {
                string userDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PastPaperHelper\\PastPaperHelper";
                if (!Directory.Exists(userDataFolderPath)) Directory.CreateDirectory(userDataFolderPath);

                XmlDocument doc = PastPaperHelperCore.Source.SaveDataToXml();
                doc.Save($"{userDataFolderPath}\\{source}.xml");
            });
            Properties.Settings.Default.FirstRun = false;
            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
        }
        #endregion

        #region LoadSubjectsCommand
        private DelegateCommand<string> _loadSubjectsCommand;
        public DelegateCommand<string> LoadSubjectsCommand =>
            _loadSubjectsCommand ?? (_loadSubjectsCommand = new DelegateCommand<string>(ExecuteLoadSubjectsCommand));

        async void ExecuteLoadSubjectsCommand(string source)
        {
            IGSubjects.Clear();
            ALSubjects.Clear();
            PastPaperHelperCore.Source = source switch
            {
                "gce_guide" => new PaperSourceGCEGuide(),
                "papacambridge" => new PaperSourcePapaCambridge(),
                "cie_notes" => new PaperSourceCIENotes(),
                _ => new PaperSourceGCEGuide(),
            };
            await PastPaperHelperUpdateService.UpdateSubjectList();
        }
        #endregion

        #region InitializeRepoCommand
        private DelegateCommand _initializeRepoCommand;
        public DelegateCommand InitializeRepoCommand =>
            _initializeRepoCommand ?? (_initializeRepoCommand = new DelegateCommand(ExecuteInitializeRepoCommand));

        async void ExecuteInitializeRepoCommand()
        {
            var lst1 = from item in IGSubjects where item.IsSelected == true select item.Subject;
            var lst2 = from item in ALSubjects where item.IsSelected == true select item.Subject;
            var lst = lst1.Union(lst2);
            IsRevertAllowed = false;
            IsProceedAllowed = false;
            TotalSubscribed = lst.Count();
            UpdateCount = 0;
            UpdateTitle = "Almost done";
            IsRetryEnabled2 = false;
            PastPaperHelperCore.Source.Subscription.Clear();
            PastPaperHelperCore.SubscribedSubjects.Clear();

            UpdateMessage = $"Initializing local files...";

            try
            {
                Task t = Task.Run(() =>
                {
                    if (Directory.Exists(Path))//TODO: disable next command if Path does not exist
                    {
                        var cachePath = $"{Path}\\.pastpaperhelper";

                        var lst = Directory.EnumerateFiles(Path, "*.pdf", SearchOption.AllDirectories);
                        Dictionary<string, string> map = new Dictionary<string, string>();
                        foreach (var path in lst)
                        {
                            string fileName = path.Split('\\').Last();
                            if (!map.ContainsKey(fileName)) map.Add(fileName, path);
                        }

                        if (!Directory.Exists(cachePath)) Directory.CreateDirectory(cachePath);
                        using (FileStream filestream = File.Create($"{cachePath}\\files.dat"))
                        {
                            BinaryFormatter serializer = new BinaryFormatter();
                            serializer.Serialize(filestream, map);
                        }
                    }
                });

                foreach (Subject subj in lst)
                {
                    UpdateMessage = $"Updating {subj.SyllabusCode} {subj.Name} from {PastPaperHelperCore.Source.DisplayName}...";
                    await PastPaperHelperUpdateService.SubscribeAsync(subj);
                    UpdateCount += 1;
                }
                await t;
            }
            catch (Exception e)
            {
                IsRetryEnabled2 = true;
                IsRevertAllowed = true;
                IsProceedAllowed = false;
                UpdateTitle = "Error";
                UpdateMessage = e.Message;
                return;
            }

            UpdateTitle = "Finished setting up Exam Revision Helper.";
            UpdateMessage = "Click \"Done\" to exit setup and restart the program.";
            IsRevertAllowed = true;
            IsProceedAllowed = true;
        }
        #endregion

        #region RetryCommand
        private DelegateCommand _retryCommand;
        public DelegateCommand RetryCommand =>
            _retryCommand ?? (_retryCommand = new DelegateCommand(ExecuteRetryCommand));

        void ExecuteRetryCommand()
        {
            _ = PastPaperHelperUpdateService.UpdateSubjectList();
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
