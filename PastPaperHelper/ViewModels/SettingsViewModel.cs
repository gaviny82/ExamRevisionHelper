using Microsoft.WindowsAPICodePack.Dialogs;
using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using PastPaperHelper.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

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
            RemoveSubjectCommand = new DelegateCommand(RemoveSubject);
            AddSelectedSubjectCommand = new DelegateCommand(AddSelectedSubject);
            BrowseCommand = new DelegateCommand(Browse);

            Path = Properties.Settings.Default.Path;
            switch (Properties.Settings.Default.PaperSource)
            {
                default: PaperSource = PaperSources.PapaCambridge; break;
                case "GCE Guide": PaperSource = PaperSources.GCE_Guide; break;
                case "PapaCambridge": PaperSource = PaperSources.PapaCambridge; break;
                case "CIE Notes": PaperSource = PaperSources.CIE_Notes; break;
            }
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
        private void RemoveSubject(object param)
        {
            Subject subject = (Subject)param;
            //TODO: hot reload

            string code = param as string;
            for (int i = 0; i < SubjectSubscribed.Count; i++)
            {
                if (SubjectSubscribed[i].SyllabusCode == code)
                {
                    SubjectSubscribed.RemoveAt(i);
                    Properties.Settings.Default.SubjectsSubcripted.Remove(code);
                    Properties.Settings.Default.Save();
                    return;
                }
            }
        }

        public DelegateCommand RemoveSelectedSubjectsCommand { get; set; }
        private void RemoveSelectedSubjects(object param)
        {
            IList list = (IList)param;
            while (list.Count > 0)
            {
                Subject subject = (Subject)list[0];
                SubjectSubscribed.Remove(subject);
                Properties.Settings.Default.SubjectsSubcripted.Remove(subject.SyllabusCode);
            }
            Properties.Settings.Default.Save();
        }

        public DelegateCommand AddSelectedSubjectCommand { get; set; }
        private void AddSelectedSubject(object param)
        {
            Dictionary<Subject, PaperRepository> item = SubscriptionManager.Subscription;
            Subject subject = (Subject)param;
            if (!SubjectSubscribed.Contains(subject))
            {
                SubjectSubscribed.Add(subject);
                Properties.Settings.Default.SubjectsSubcripted.Add(subject.SyllabusCode);
                Properties.Settings.Default.Save();
            }
        }

        public DelegateCommand BrowseCommand { get; set; }
        private void Browse(object param)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Path = dialog.FileName;
            }
            SearchViewModel model = ((Application.Current.MainWindow.DataContext as MainWindowViewModel).ListItems[0].Content as SearchView).DataContext as SearchViewModel;
            model.SearchPath = Path;
        }
    }
}
