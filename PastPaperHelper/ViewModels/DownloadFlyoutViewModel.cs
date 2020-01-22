using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using PastPaperHelper.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PastPaperHelper.ViewModels
{
    public class DownloadFlyoutViewModel : NotificationObject
    {
        public static ObservableCollection<DownloadTask> Tasks = new ObservableCollection<DownloadTask>();

        private DelegateCommand _downloadCommand;
        public DelegateCommand DownloadCommand =>
            _downloadCommand ?? (_downloadCommand = new DelegateCommand(ExecuteDownloadCommand));

        private DelegateCommand _logCommand;
        public DelegateCommand LogCommand =>
            _logCommand ?? (_logCommand = new DelegateCommand(ExecuteLogCommand));

        void ExecuteLogCommand(object param)
        {
            Task.Factory.StartNew(() =>
            {
                Log += $"\n[{DateTime.Now.ToShortTimeString()}] {param.ToString()}";
            }, new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
        }

        private int _totalTasks;
        public int TotalTasks
        {
            get { return _totalTasks; }
            set { _totalTasks = value; RaisePropertyChangedEvent(nameof(TotalTasks)); }
        }

        private int _taskCompleted;
        public int TaskCompleted
        {
            get { return _taskCompleted; }
            set { _taskCompleted = value; RaisePropertyChangedEvent(nameof(TaskCompleted)); }
        }

        private string _log;
        public string Log
        {
            get { return _log; }
            set { _log = value; RaisePropertyChangedEvent(nameof(Log)); }
        }

        private string _message = "Waiting for download...";
        public string Message
        {
            get { return _message; }
            set { _message = value; RaisePropertyChangedEvent(nameof(Message)); }
        }

        public DownloadFlyoutViewModel()
        {
            MainWindow.DownloadFlyoutViewModel = this;
            Log = $"[{DateTime.Now.ToShortTimeString()}] Download service initialized.";
        }

        void ExecuteDownloadCommand(object param)
        {
            if (!(param is Subject)) return;

            Subject subj = (Subject)param;
            PaperRepository repo = SubscriptionManager.Subscription[subj];
            TotalTasks = 0;
            TaskCompleted = 0;
            ExecuteLogCommand("Initializing download task...");

            string path = Properties.Settings.Default.Path;
            path += $"\\{repo.Subject.SyllabusCode} {(repo.Subject.Curriculum == Curriculums.ALevel ? "AL" : "GCSE")} {repo.Subject.Name}";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            foreach (ExamYear year in repo)
            {
                if (year.Specimen != null) DownloadPaper(year.Specimen, path);
                if (year.Spring != null) DownloadPaper(year.Spring, path);
                if (year.Summer != null) DownloadPaper(year.Summer, path);
                if (year.Winter != null) DownloadPaper(year.Winter, path);
            }
            ExecuteLogCommand($"{Tasks.Count} task{(Tasks.Count>1?"s":"")} enqueued.");
            ExecuteLogCommand($"Subject Downloading: {subj.SyllabusCode} {subj.Name}");
            
            //Task.Run(() =>
            //{
            //    int part = downloadTasks.Count / 5;
            //    Task.Factory.StartNew(() =>
            //    {
            //        TotalTasks = downloadTasks.Count;
            //    }, new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
            //    download(0, part, client1);
            //    download(part, part * 2, client2);
            //    download(part * 2, part * 3, client3);
            //    download(part * 3, part * 4, client4);
            //    download(part * 4, downloadTasks.Count, client5);
            //});
        }


        //private void download(int start, int end, WebClient client)
        //{
        //    Task.Run(() =>
        //    {
        //        for (int i = start; i < end; i++)
        //        {
        //            try
        //            {
        //                client.DownloadFile(downloadTasks[i].Item1, downloadTasks[i].Item2);
        //            }
        //            catch (System.Exception)
        //            {
        //                var index = i;
        //                failedTasks.Add(downloadTasks[index]);
        //                Task.Factory.StartNew(() =>
        //                {
        //                    Log += "\n" + downloadTasks[index].Item1;
        //                }, new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
        //                continue;
        //            }
        //            finally
        //            {
        //                Task.Factory.StartNew(() =>
        //                {
        //                    TaskCompleted += 1;
        //                }, new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
        //            }
        //        }
        //    });
        //}

        WebClient client1 = new WebClient();
        WebClient client2 = new WebClient();
        WebClient client3 = new WebClient();
        WebClient client4 = new WebClient();
        WebClient client5 = new WebClient();
        private void DownloadPaper(Exam exam, string dir)
        {
            string ser = "";
            switch (exam.Series)
            {
                case ExamSeries.Spring:
                    ser = "March";
                    break;
                case ExamSeries.Summer:
                    ser = "May-June";
                    break;
                case ExamSeries.Winter:
                    ser = "Oct-Nov";
                    break;
                case ExamSeries.Specimen:
                    ser = "Specimen";
                    break;
                default:
                    break;
            }
            dir += $"\\{exam.Year} {ser}";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var lst = (from comp in exam.Components select comp.Variants);
            foreach (var varients in lst)
            {
                foreach (var item in varients)
                {
                    foreach (Paper paper in item.Papers)
                    {
                        string file = paper.Url.Split('/').Last();
                        Tasks.Add(new DownloadTask
                        {
                            FileName = file,
                            State = DownloadTaskState.Pending,
                            Progress = 0,
                            ResourceUrl = paper.Url,
                            LocalPath = $"{dir}\\{file}",
                        });
                    }
                }
            }
        }
    }
}
