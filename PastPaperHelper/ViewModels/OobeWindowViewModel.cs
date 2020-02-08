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

        public OobeWindowViewModel()
        {
            PastPaperHelperUpdateService.UpdateServiceNotifiedEvent += (args) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (args.NotificationType == NotificationType.Initializing)
                    {
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

            PastPaperHelperUpdateService.UpdateServiceErrorEvent += (args) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    //TODO: Show retry button
                });
            };

            var task = PastPaperHelperUpdateService.UpdateSubjectList();
        }

        private string _path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Past Papers";
        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value); }
        }

        //private Visibility _isLoading;
        //public Visibility IsLoading
        //{
        //    get { return _isLoading; }
        //    set { _isLoading = value; RaisePropertyChangedEvent("IsLoading"); }
        //}

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
            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
            Process.Start(Environment.CurrentDirectory + "\\PastPaperHelper.exe");
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
