using Microsoft.WindowsAPICodePack.Dialogs;
using PastPaperHelper.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PastPaperHelper.PrismTest.ViewModels
{
    public class OobeWindowViewModel : BindableBase
    {
        public ObservableCollection<SubjectSelection> IGSubjects { get; set; } = new ObservableCollection<SubjectSelection>();
        public ObservableCollection<SubjectSelection> ALSubjects { get; set; } = new ObservableCollection<SubjectSelection>();

        public OobeWindowViewModel()
        {
            Task.Factory.StartNew(() =>
            {
                //SubscriptionManager.UpdateAndInit(true, false);
            }).ContinueWith(t =>
            {
                IGSubjects.Clear();
                ALSubjects.Clear();
                //foreach (Subject item in SubscriptionManager.AllSubjects)
                //{
                //    if (item.Curriculum == Curriculums.IGCSE) IGSubjects.Add(new SubjectSelection(item, false));
                //    else ALSubjects.Add(new SubjectSelection(item, false));
                //}
                //IsLoading = Visibility.Hidden;
            }, TaskScheduler.FromCurrentSynchronizationContext());
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
            Process.Start(Environment.CurrentDirectory + "/PastPaperHelper.exe");
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
