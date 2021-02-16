using ExamRevisionHelper.Core.Tools;
using ExamRevisionHelper.Models;
using ExamRevisionHelper.Views;
using ExamRevisionHelper.Sources;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Ookii.Dialogs.Wpf;

namespace ExamRevisionHelper.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public SettingsViewModel()
        {
            _updateFrequency = (UpdateFrequency)Properties.Settings.Default.UpdatePolicy;
            _paperSource = Properties.Settings.Default.PaperSource;
            _path = Properties.Settings.Default.Path;
            _autoUpdateFileList = Properties.Settings.Default.AutoUpdateFileList;
        }

        #region Setting: PaperSource
        private string _paperSource;
        public string PaperSource
        {
            get { return _paperSource; }
            set { SetProperty(ref _paperSource, value); PaperSourceChanged(value); }
        }

        private void PaperSourceChanged(string value)
        {
            Properties.Settings.Default.PaperSource = value;
            Properties.Settings.Default.Save();
        }
        #endregion

        #region Setting: Path
        private string _path;
        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value); }
        }
        #endregion

        #region Setting: UpdateFrequency
        private UpdateFrequency _updateFrequency;
        public UpdateFrequency UpdateFrequency
        {
            get { return _updateFrequency; }
            set { SetProperty(ref _updateFrequency, value); UpdateFrequencyChanged(value); }
        }

        private void UpdateFrequencyChanged(UpdateFrequency value)
        {
            Properties.Settings.Default.UpdatePolicy = (int)value;
            Properties.Settings.Default.Save();
        }
        #endregion

        private bool _autoUpdateFileList;
        public bool AutoUpdateFileList
        {
            get { return _autoUpdateFileList; }
            set { SetProperty(ref _autoUpdateFileList, value); }
        }
        //TODO: implement this option

        #region RemoveSubjectCommand
        private DelegateCommand<Subject> _removeSubjectCommand;
        public DelegateCommand<Subject> RemoveSubjectCommand =>
            _removeSubjectCommand ?? (_removeSubjectCommand = new DelegateCommand<Subject>(ExecuteRemoveSubjectCommand));

        void ExecuteRemoveSubjectCommand(Subject subj)
        {
            PastPaperHelperUpdateService.Unsubscribe(subj);
            MainWindowViewModel.RefreshSubscribedSubjects();
        }
        #endregion

        #region RemoveSelectedSubjectsCommand
        private DelegateCommand<IList> _removeSelectedSubjectsCommand;
        public DelegateCommand<IList> RemoveSelectedSubjectsCommand =>
            _removeSelectedSubjectsCommand ?? (_removeSelectedSubjectsCommand = new DelegateCommand<IList>(ExecuteRemoveSelectedSubjectsCommand));

        void ExecuteRemoveSelectedSubjectsCommand(IList list)
        {
            while (list.Count > 0)
            {
                Subject subject = (Subject)list[0];
                ExecuteRemoveSubjectCommand(subject);
            }
        }
        #endregion

        #region BrowseCommand
        private DelegateCommand _browseCommand;
        public DelegateCommand BrowseCommand =>
            _browseCommand ?? (_browseCommand = new DelegateCommand(ExecuteBrowseCommand));

        void ExecuteBrowseCommand()
        {
            VistaFolderBrowserDialog picker = new();
            if (picker.ShowDialog() == true)
            {
                var path = picker.SelectedPath;
                Path = path;
                Properties.Settings.Default.Path = path;
                Properties.Settings.Default.Save();
                PastPaperHelperCore.LocalFilesPath = path;
            }
        }
        #endregion

        private DelegateCommand _updateAllCommand;
        public DelegateCommand UpdateAllCommand =>
            _updateAllCommand ?? (_updateAllCommand = new DelegateCommand(ExecuteUpdateAllCommand));

        void ExecuteUpdateAllCommand()
        {
            var lst = (from subj in PastPaperHelperCore.SubscribedSubjects select subj.SyllabusCode).ToList();
            PastPaperHelperUpdateService.UpdateAll(lst);
        }
    }
}
