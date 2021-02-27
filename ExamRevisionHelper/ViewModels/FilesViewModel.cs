using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using ExamRevisionHelper.Core;
using ExamRevisionHelper.Core.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace ExamRevisionHelper.ViewModels
{
    public class FilesViewModel : BindableBase
    {
        public static readonly Exam EmptyExam = new Exam();

        private Task CompareLocalFilesToSource;

        public FilesViewModel()
        {
            SelectedExamSeries = EmptyExam;
            InitFileListCache();
        }

        private async void InitFileListCache()
        {
            var fileListCachePath = App.CurrentInstance.LocalFileStorage.FullName + "\\.pastpaperhelper";
            if (!Directory.Exists(fileListCachePath))
            {
                Directory.CreateDirectory(fileListCachePath);
                File.SetAttributes(fileListCachePath, FileAttributes.Hidden);
            }
            string cacheFile = $"{fileListCachePath}\\files.dat";

            Task load = Task.Run(() =>
            {
                if (File.Exists(cacheFile))
                {
                    using (FileStream fileStream = File.OpenRead(cacheFile))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        ExamRevisionHelperCore.LocalFiles = (Dictionary<string, string>)binaryFormatter.Deserialize(fileStream);
                    }
                }
                else
                {
                    ExamRevisionHelperCore.LocalFiles = new Dictionary<string, string>();
                }
            });

            Dictionary<string, string> newMap = new Dictionary<string, string>();
            CompareLocalFilesToSource = Task.Run(() =>
            {
                //Load current files list
                var lst = App.CurrentInstance.LocalFileStorage.EnumerateFiles("*.pdf", SearchOption.AllDirectories);
                foreach (var file in lst)
                {
                    string fileName = file.Name;
                    if (!newMap.ContainsKey(fileName)) newMap.Add(fileName, file.FullName);
                }
            });

            //Compare new files list to the stored one
            await load;
            await CompareLocalFilesToSource;
            bool newEntry = false;
            if (ExamRevisionHelperCore.LocalFiles == null) { newEntry = true; }
            else
            {
                foreach (var item in newMap)
                {
                    if (!ExamRevisionHelperCore.LocalFiles.ContainsKey(item.Key) || ExamRevisionHelperCore.LocalFiles[item.Key] != item.Value)
                    {
                        newEntry = true;
                        break;
                    }
                }
                if (!newEntry)
                {
                    foreach (var item in ExamRevisionHelperCore.LocalFiles)
                    {
                        if (!newMap.ContainsKey(item.Key))
                        {
                            newEntry = true;
                            break;
                        }
                    }
                }
            }

            if (newEntry)
            {
                ExamRevisionHelperCore.LocalFiles = newMap;
                using (FileStream filestream = File.Create(cacheFile))
                {
                    BinaryFormatter serializer = new BinaryFormatter();
                    serializer.Serialize(filestream, newMap);
                }
            }
        }


        private Exam _selectedExamSeries;
        public Exam SelectedExamSeries
        {
            get { return _selectedExamSeries; }
            set { SetProperty(ref _selectedExamSeries, value); }
        }

        #region OpenExamSeriesCommand
        private DelegateCommand<Exam> _openExamSeriesCommand;
        public DelegateCommand<Exam> OpenExamSeriesCommand =>
            _openExamSeriesCommand ?? (_openExamSeriesCommand = new DelegateCommand<Exam>(ExecuteCommandName));

        void ExecuteCommandName(Exam exam)
        {
            SelectedExamSeries = exam;
        }
        #endregion

        #region OpenResourcesCommand
        private DelegateCommand<PastPaperResource> _openResourceCommand;
        public DelegateCommand<PastPaperResource> OpenResourcesCommand =>
            _openResourceCommand ?? (_openResourceCommand = new DelegateCommand<PastPaperResource>(ExecuteOpenResource));

        private async void ExecuteOpenResource(PastPaperResource resource)
        {
            var filename = resource.Url?.Split('/').Last();

            if (ExamRevisionHelperCore.LocalFiles.ContainsKey(filename))
            {
                var file = ExamRevisionHelperCore.LocalFiles[filename];
                if (File.Exists(file)) App.StartProcess(file);
            }
            else
            {
                if (!CompareLocalFilesToSource.IsCompleted)
                {
                    await CompareLocalFilesToSource;
                    if (ExamRevisionHelperCore.LocalFiles.ContainsKey(filename))
                    {
                        var file = ExamRevisionHelperCore.LocalFiles[filename];
                        if (File.Exists(file)) App.StartProcess(file);
                    }
                }
                App.StartProcess(resource.Url);
            }
            //Process.Start(resource.State == ResourceStates.Offline && !string.IsNullOrEmpty(resource.Path) ?
            //resource.Path :
            //resource.Url);
        }
        #endregion

        private DelegateCommand<Variant> _openPaperCommand;
        public DelegateCommand<Variant> OpenPaperCommand =>
            _openPaperCommand ?? (_openPaperCommand = new DelegateCommand<Variant>(ExecuteOpenPaperCommand));

        void ExecuteOpenPaperCommand(Variant parameter)
        {
            if (parameter == null) return;
            foreach (Paper item in parameter.Papers)
            {
                var filename = item.Url?.Split('/').Last();
                if (item.Type == ResourceType.QuestionPaper)
                {
                    Process.Start(ExamRevisionHelperCore.LocalFiles[filename]);
                }
                else if (item.Type == ResourceType.Insert)
                {
                    Process.Start(ExamRevisionHelperCore.LocalFiles[filename]);
                }
                else if (item.Type == ResourceType.ListeningAudio)
                {
                    Process.Start(ExamRevisionHelperCore.LocalFiles[filename]);
                }
            }
        }
    }
}
