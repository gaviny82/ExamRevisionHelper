using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace PastPaperHelper
{
    class SettingsViewModel : ObservableObject
    {
        public SettingsViewModel()
        {
            Path = Properties.Settings.Default.Path;
            PaperSource = (PastPaperSources)Properties.Settings.Default.PaperSource;
            AutoUpdateFiles = Properties.Settings.Default.AutoUpdateFiles;
            AutoUpdateProgram = Properties.Settings.Default.AutoUpdateProgram;

            if(App.SubjectList!=null)
                SubjectsLastUpdate = DateTime.Parse(App.SubjectList.ChildNodes[1].Attributes["Time"].Value);

            Subjects.Add(new Subject { Curriculum = Curriculums.IGCSE, Name = "Physics", SyllabusCode = "0625" });
            Subjects.Add(new Subject { Curriculum = Curriculums.ALevel, Name = "Mathematics", SyllabusCode = "9709" });
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

        private PastPaperSources _paperSource;
        public PastPaperSources PaperSource
        {
            get { return _paperSource; }
            set
            {
                _paperSource = value;
                RaisePropertyChangedEvent("PaperSource");
                Properties.Settings.Default.PaperSource = (int)value;
                Properties.Settings.Default.Save();//TODO: show url in subject management view
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

        public ICommand RemoveSubjectCommand
        {
            get => new DelegateCommand(RemoveSubject);
        }
        private void RemoveSubject(object param)
        {
            string code = param as string;
            for (int i = 0; i < Subjects.Count; i++)
            {
                if (Subjects[i].SyllabusCode == code)
                {
                    Subjects.RemoveAt(i);
                    return;
                }
            }
        }

        public ICommand RemoveSelectedSubjectsCommand
        {
            get => new DelegateCommand(RemoveSelectedSubjects);
        }
        private void RemoveSelectedSubjects(object param)
        {
            IList list = (IList)param;
            while (list.Count > 0)
            {
                Subjects.Remove((Subject)list[0]);
            }
        }

        public ObservableCollection<Subject> Subjects { get; } = new ObservableCollection<Subject>();
    }
}
