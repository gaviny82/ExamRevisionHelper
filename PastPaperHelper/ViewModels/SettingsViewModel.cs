using Microsoft.WindowsAPICodePack.Dialogs;
using PastPaperHelper.Sources;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace PastPaperHelper
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

            //if(App.SubjectList!=null)
            //    SubjectsLastUpdate = DateTime.Parse(App.SubjectList.ChildNodes[1].Attributes["Time"].Value);

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
                SourceManager.CurrentPaperSource = value;
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
