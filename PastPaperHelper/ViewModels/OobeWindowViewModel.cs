using Microsoft.WindowsAPICodePack.Dialogs;
using PastPaperHelper.Core.Tools;
using PastPaperHelper.Events;
using PastPaperHelper.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PastPaperHelper.ViewModels
{
    public class OobeWindowViewModel : BindableBase
    {
        public ObservableCollection<SubjectSelection> IGSubjects { get; set; } = new ObservableCollection<SubjectSelection>();
        public ObservableCollection<SubjectSelection> ALSubjects { get; set; } = new ObservableCollection<SubjectSelection>();

        public OobeWindowViewModel(IEventAggregator eventAggregator)
        {
            Init(eventAggregator);
        }

        public async void Init(IEventAggregator eventAggregator)
        {
            PastPaperHelperUpdateService.UpdateErrorEvent += (msg) => { eventAggregator.GetEvent<UpdateServiceErrorEvent>().Publish(); };
            PastPaperHelperUpdateService.UpdateFinalizedEvent += () => { eventAggregator.GetEvent<SubjectListDownloadedEvent>().Publish(); };
            Application.Current.Resources["IsLoading"] = Visibility.Visible;

            eventAggregator.GetEvent<UpdateServiceErrorEvent>().Subscribe(() =>
            {

            }, ThreadOption.UIThread);

            eventAggregator.GetEvent<SubjectListDownloadedEvent>().Subscribe(() =>
            {
                Application.Current.Resources["IsLoading"] = Visibility.Hidden;
                foreach (var item in PastPaperHelperCore.SubjectsLoaded)
                {
                    if (item.Curriculum == Curriculums.ALevel) ALSubjects.Add(new SubjectSelection(item, false));
                    else IGSubjects.Add(new SubjectSelection(item, false));
                }
            }, ThreadOption.UIThread);

            await PastPaperHelperUpdateService.UpdateSubjectList();
        }

        private string _path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Past Papers";
        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value); }
        }

        //public string Path
        //{
        //    get { return _path; }
        //    set
        //    {
        //        _path = value;
        //        RaisePropertyChangedEvent("Path");
        //        if (Directory.Exists(value))
        //        {
        //            Properties.Settings.Default.Path = value;
        //            Properties.Settings.Default.Save();
        //        }
        //    }
        //}

        //private Visibility _isLoading;
        //public Visibility IsLoading
        //{
        //    get { return _isLoading; }
        //    set { _isLoading = value; RaisePropertyChangedEvent("IsLoading"); }
        //}

        private DelegateCommand _browseCommand;
        public DelegateCommand BrowseCommand =>
            _browseCommand ?? (_browseCommand = new DelegateCommand(ExecuteBrowseCommand));

        void ExecuteBrowseCommand()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true })
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Path = dialog.FileName + "\\Past Papers";
                }
            }
        }

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
            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
            Process.Start(Environment.CurrentDirectory + "\\PastPaperHelper.exe");
        }

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
