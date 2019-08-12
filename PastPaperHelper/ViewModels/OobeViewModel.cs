using Microsoft.WindowsAPICodePack.Dialogs;
using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PastPaperHelper.ViewModels
{
    class OobeViewModel : NotificationObject
    {
        public OobeViewModel()
        {
            SaveSettingsCommand = new DelegateCommand(SaveSettings);
            SaveCommand = new DelegateCommand(Save);

            bool updateSubjectList = false, updateSubscription = false;
            Task.Factory.StartNew(() =>
            {
                SubscriptionManager.CheckUpdate(out updateSubjectList, out updateSubscription);
                SubscriptionManager.UpdateAndInit(updateSubjectList, updateSubscription);
            }).ContinueWith(t =>
            {
                if (SubscriptionManager.AllSubjects != null)
                {
                    IGSubjects.Clear();
                    ALSubjects.Clear();
                    foreach (Subject item in SubscriptionManager.AllSubjects)
                    {
                        if (item.Curriculum == Curriculums.ALevel)
                            ALSubjects.Add(new SubjectSelection(item, false));
                        else
                            IGSubjects.Add(new SubjectSelection(item, false));
                    }
                    IsLoading = Visibility.Hidden;
                    //((DataContext as MainWindowViewModel).ListItems[1].Content as FilesView).UpdateSelectedItem();
                }
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


        public DelegateCommand SaveSettingsCommand { get; set; }
        private void SaveSettings(object param)
        {

            //DEBUG:
            //PastPaperHelper.Properties.Settings.Default.FirstRun = false;
            //PastPaperHelper.Properties.Settings.Default.Save();
        }


        private Visibility _isLoading;
        public Visibility IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; RaisePropertyChangedEvent("IsLoading"); }
        }


        public ICommand BrowseCommand
        {
            get => new DelegateCommand(Browse);
        }
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

        public ObservableCollection<SubjectSelection> IGSubjects { get; set; } = new ObservableCollection<SubjectSelection>();
        public ObservableCollection<SubjectSelection> ALSubjects { get; set; } = new ObservableCollection<SubjectSelection>();
    }

    class SubjectSelection
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
