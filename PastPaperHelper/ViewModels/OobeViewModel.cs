using Microsoft.WindowsAPICodePack.Dialogs;
using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace PastPaperHelper.ViewModels
{
    class OobeViewModel : NotificationObject
    {
        public ObservableCollection<SubjectSelection> IGSubjects { get; set; } = new ObservableCollection<SubjectSelection>();
        public ObservableCollection<SubjectSelection> ALSubjects { get; set; } = new ObservableCollection<SubjectSelection>();
        public OobeViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            BrowseCommand = new DelegateCommand(Browse);

            Task.Factory.StartNew(() =>
            {
                SubscriptionManager.UpdateAndInit(true, false);
            }).ContinueWith(t =>
            {
                IGSubjects.Clear();
                ALSubjects.Clear();
                foreach (Subject item in SubscriptionManager.AllSubjects)
                {
                    if (item.Curriculum == Curriculums.IGCSE) IGSubjects.Add(new SubjectSelection(item, false));
                    else ALSubjects.Add(new SubjectSelection(item, false));
                }
                IsLoading = Visibility.Hidden;
            }, TaskScheduler.FromCurrentSynchronizationContext());
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

        private Visibility _isLoading;
        public Visibility IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; RaisePropertyChangedEvent("IsLoading"); }
        }


        public DelegateCommand BrowseCommand { get; set; }
        private void Browse(object param)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Path = dialog.FileName;
            }
        }


        public DelegateCommand SaveCommand { get; set; }
        private void Save(object param)
        {
            if (!Directory.Exists(Path)) return;

            Properties.Settings.Default.Path = Path;
            Properties.Settings.Default.SubjectsSubcripted.Clear();
            foreach (SubjectSelection item in IGSubjects)
            {
                if (item.IsSelected == true) Properties.Settings.Default.SubjectsSubcripted.Add(item.Subject.SyllabusCode);
            }
            foreach (SubjectSelection item in ALSubjects)
            {
                if (item.IsSelected == true) Properties.Settings.Default.SubjectsSubcripted.Add(item.Subject.SyllabusCode);
            }
            Properties.Settings.Default.FirstRun = false;
            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
            Process.Start(Environment.CurrentDirectory + "/PastPaperHelper.exe");
        }
    }

    internal class SubjectSelection
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
