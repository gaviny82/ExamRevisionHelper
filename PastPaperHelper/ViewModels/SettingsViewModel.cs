using Microsoft.WindowsAPICodePack.Dialogs;
using PastPaperHelper.Core.Tools;
using PastPaperHelper.Models;
using PastPaperHelper.Views;
using PastPaperHelper.Sources;
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

namespace PastPaperHelper.ViewModels
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
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true })
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Path = dialog.FileName;
                    Properties.Settings.Default.Path = dialog.FileName;
                    Properties.Settings.Default.Save();
                }
                else return;
            }
        }
        #endregion

    }
}
