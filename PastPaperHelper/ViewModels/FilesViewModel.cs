using PastPaperHelper.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Xml;

namespace PastPaperHelper.ViewModels
{
    public class FilesViewModel : BindableBase
    {
        public static readonly Exam EmptyExam = new Exam();

        public readonly Task CompareLocalFilesToSource;

        public static Dictionary<string, string> LocalFiles;

        public FilesViewModel()
        {
            SelectedExamSeries = EmptyExam;

            //Load file dictionary cache
            //if (File.Exists($"{Properties.Settings.Default.Path}\\.pastpaperhelper\\files.xml"))
            //{
            //    XmlDocument doc = new XmlDocument();
            //    doc.Load($"{Properties.Settings.Default.Path}\\.pastpaperhelper\\files.xml");
            //    foreach (XmlNode item in doc.SelectNodes("//File"))
            //    {
            //        string name = item.Attributes["Name"].Value;
            //        string path = item.Attributes["Path"].Value;
            //        if (!LocalFiles.ContainsKey(name))
            //            LocalFiles.Add(name, path);
            //    }
            //}
            string cacheFile = $"{Properties.Settings.Default.Path}\\.pastpaperhelper\\files.dat";
            if (File.Exists(cacheFile))
            {
                using (FileStream fileStream = File.OpenRead(cacheFile))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    LocalFiles = (Dictionary<string, string>)binaryFormatter.Deserialize(fileStream);
                }
            }

            CompareLocalFilesToSource = Task.Run(() =>
            {
                bool newEntry = false;
                string dirPath = PastPaperHelper.Properties.Settings.Default.Path;

                var lst = Directory.EnumerateFiles(dirPath, "*.pdf", SearchOption.AllDirectories);
                Dictionary<string, string> newMap = new Dictionary<string, string>();
                foreach (var path in lst)
                {
                    string fileName = path.Split('\\').Last();
                    if (!newMap.ContainsKey(fileName)) newMap.Add(fileName, path);
                }

                foreach (var item in newMap)
                {
                    if (!LocalFiles.ContainsKey(item.Key)|| LocalFiles[item.Key] != item.Value)
                    {
                        newEntry = true;
                        break;
                    }
                }
                if (!newEntry)
                {
                    foreach (var item in LocalFiles)
                    {
                        if (!newMap.ContainsKey(item.Key))
                        {
                            newEntry = true;
                            break;
                        }
                    }
                }

                if (newEntry)
                {
                    LocalFiles = newMap;
                    using (FileStream filestream = File.Create(cacheFile))
                    {
                        BinaryFormatter serializer = new BinaryFormatter();
                        serializer.Serialize(filestream, newMap);
                    }
                }

            });
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

        private void ExecuteOpenResource(PastPaperResource resource)
        {
            var filename = resource.Url?.Split('/').Last();
            if (LocalFiles.ContainsKey(filename))
            {
                var file = LocalFiles[filename];
                if (File.Exists(file))Process.Start(file);
                return;
            }
            Process.Start(resource.State == ResourceStates.Offline && !string.IsNullOrEmpty(resource.Path) ?
            resource.Path :
            resource.Url);
        }
        #endregion
    }
}
