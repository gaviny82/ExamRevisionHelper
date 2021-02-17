using AsyncAwaitBestPractices;
using ExamRevisionHelper.Models;
using ExamRevisionHelper.Sources;
using Prism.Commands;
using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace ExamRevisionHelper.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public static ObservableCollection<Subject> SubscribedSubjects { get; } = new ObservableCollection<Subject>(App.SubscribedSubjects);

        static SettingsPageViewModel()
        {
            SubjectSubscriptionUtils.SubjectSubscribedEvent += (subject) => { SubscribedSubjects.Add(subject); };
            SubjectSubscriptionUtils.SubjectUnsubscribedEvent += (subject) => { SubscribedSubjects.Remove(subject); };
        }

        #region Setting: PaperSource
        private string _paperSource = App.PaperSource.Name;
        public string PaperSource
        {
            get { return _paperSource; }
            set
            {
                SetProperty(ref _paperSource, value);
                //TODO: Hot reload paper source
                //Current: Restart to reload paper source
                App.LocalSettings.Values["PaperSource"] = value;
            }
        }
        #endregion

        #region Setting: FilesPath
        private string _filesPath = App.PastPapersFolder?.Path;
        public string FilesPath
        {
            get { return _filesPath; }
            set { SetProperty(ref _filesPath, value); }
        }
        #endregion

        #region Setting: UpdateFrequency
        private UpdateFrequency _updateFrequency = App.UpdateFrequency;
        public UpdateFrequency UpdateFrequency
        {
            get { return _updateFrequency; }
            set
            {
                SetProperty(ref _updateFrequency, value);
                App.UpdateFrequency = value;
                App.LocalSettings.Values["UpdateFrequency"] = (int)value;
            }
        }
        #endregion


        #region RemoveSubjectCommand
        private DelegateCommand<Subject> _removeSubjectCommand;
        public DelegateCommand<Subject> RemoveSubjectCommand =>
            _removeSubjectCommand ?? (_removeSubjectCommand = new DelegateCommand<Subject>(ExecuteRemoveSubjectCommand));

        void ExecuteRemoveSubjectCommand(Subject subj)
        {
            SubjectSubscriptionUtils.Unsubscribe(subj);
            //MainWindowViewModel.RefreshSubscribedSubjects();
        }
        #endregion

        #region UnsubscribeSelectedSubjectsCommand
        private DelegateCommand<IList<object>> _unsubscribeSelectedSubjectsCommand;
        public DelegateCommand<IList<object>> UnsubscribeSelectedSubjectsCommand =>
            _unsubscribeSelectedSubjectsCommand ?? (_unsubscribeSelectedSubjectsCommand = new DelegateCommand<IList<object>>(ExecuteUnsubscribeSelectedSubjectsCommand));

        void ExecuteUnsubscribeSelectedSubjectsCommand(IList<object> list)
        {
            IEnumerable<Subject> subjSelected = list.Cast<Subject>();
            foreach (var item in subjSelected)
            {
                ExecuteRemoveSubjectCommand(item);
            }
        }
        #endregion

        #region BrowseCommand
        private DelegateCommand _browseCommand;
        public DelegateCommand BrowseCommand =>
            _browseCommand ?? (_browseCommand = new DelegateCommand(ExecuteBrowseCommand));

        async void ExecuteBrowseCommand()
        {
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PastPapersFolder", folder);

                FilesPath= folder.Path;
                App.PastPapersFolder = folder;
                App.LocalSettings.Values["FilesPath"] = folder.Path;
            }
        }
        #endregion

        #region UpdateDataCommand
        private DelegateCommand _updateDataCommand;
        public DelegateCommand UpdateDataCommand =>
            _updateDataCommand ?? (_updateDataCommand = new DelegateCommand(ExecuteUpdateDataCommand));

        void ExecuteUpdateDataCommand()
        {
            var lst = (from subj in App.SubscribedSubjects select subj.SyllabusCode).ToList();
            SubjectSubscriptionUtils.UpdateDataAsync(App.PaperSource, lst).SafeFireAndForget();
        }
        #endregion
    }
}
