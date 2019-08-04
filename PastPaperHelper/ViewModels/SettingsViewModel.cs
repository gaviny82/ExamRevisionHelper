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
        public SettingsViewModel()
        {
            Path = Properties.Settings.Default.Path;
            switch (Properties.Settings.Default.PaperSource)
            {
                default: PaperSource = PaperSources.GCE_Guide; break;
                case "GCE Guide": PaperSource = PaperSources.GCE_Guide; break;
                case "PapaCambridge": PaperSource = PaperSources.PapaCambridge; break;
                case "CIE Notes": PaperSource = PaperSources.CIE_Notes; break;
            }
            AutoUpdateFiles = Properties.Settings.Default.AutoUpdateFiles;
            AutoUpdateProgram = Properties.Settings.Default.AutoUpdateProgram;


            RemoveSelectedSubjectsCommand = new DelegateCommand(RemoveSelectedSubjects);
            RemoveSubjectCommand = new DelegateCommand(RemoveSubject);
            AddSelectedSubjectCommand = new DelegateCommand(AddSelectedSubject);
        }

        //TODO: Global subject subscription management
        public static ObservableCollection<Subject> SubjectsSubscripted { get; } = new ObservableCollection<Subject>();
        public static ObservableCollection<Subject> IGSubjects { get; } = new ObservableCollection<Subject>();
        public static ObservableCollection<Subject> ALSubjects { get; } = new ObservableCollection<Subject>();

        private Subject _selectedSubject;
        public Subject SelectedSubject
        {
            get { return _selectedSubject; }
            set { _selectedSubject = value; RaisePropertyChangedEvent("SelectedSubject"); }
        }

        public DelegateCommand RemoveSubjectCommand { get; set; }
        private void RemoveSubject(object param)
        {
            string code = param as string;
            for (int i = 0; i < SubjectsSubscripted.Count; i++)
            {
                if (SubjectsSubscripted[i].SyllabusCode == code)
                {
                    SubjectsSubscripted.RemoveAt(i);
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
                SubjectsSubscripted.Remove(subject);
                Properties.Settings.Default.SubjectsSubcripted.Remove(subject.SyllabusCode);
            }
            Properties.Settings.Default.Save();
        }


        public DelegateCommand AddSelectedSubjectCommand { get; set; }
        private void AddSelectedSubject(object param)
        {
            Dictionary<Subject, PaperRepository> item = SubscriptionManager.Subscription;
            //TODO: Hot reload papers at download view
            Subject subject = (Subject)param;
            if (!SubjectsSubscripted.Contains(subject))
            {
                SubjectsSubscripted.Add(subject);
                Properties.Settings.Default.SubjectsSubcripted.Add(subject.SyllabusCode);
                Properties.Settings.Default.Save();
            }
        }

        public static void RefreshSubjectList()
        {
            if (SubscriptionManager.AllSubjects != null)
            {
                IGSubjects.Clear();
                ALSubjects.Clear();
                foreach (Subject item in SubscriptionManager.AllSubjects)
                {
                    if (item.Curriculum == Curriculums.ALevel)
                        ALSubjects.Add(item);
                    else
                        IGSubjects.Add(item);
                }
            }

            SubjectsSubscripted.Clear();
            foreach (KeyValuePair<Subject, PaperRepository> item in SubscriptionManager.Subscription)
            {
                if (SubscriptionManager.TryFindSubject(item.Key.SyllabusCode, SubscriptionManager.AllSubjects, out Subject result))
                    SubjectsSubscripted.Add(result);
            }

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

        private DateTime _subjectsLastUpdate;
        public DateTime SubjectsLastUpdate
        {
            get { return _subjectsLastUpdate; }
            set
            {
                _subjectsLastUpdate = value;
                RaisePropertyChangedEvent("SubjectsLastUpdate");
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
                SubscriptionManager.CurrentPaperSource = value;
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
            SearchViewModel model = ((Application.Current.MainWindow.DataContext as MainWindowViewModel).ListItems[0].Content as SearchView).DataContext as SearchViewModel;
            model.SearchPath = Path;
        }
    }
}
