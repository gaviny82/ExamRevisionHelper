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

        private bool _isIndeterminate = true;
        public bool IsIndeterminate
        {
            get { return _isIndeterminate; }
            set { _isIndeterminate = value; RaisePropertyChangedEvent(nameof(IsIndeterminate)); }
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

        async void ExecuteDownloadCommand(object param)
        {
            if (!(param is Subject)) return;

            Subject subj = (Subject)param;
            PaperRepository repo = SubscriptionManager.Subscription[subj];
            TotalTasks = 0;
            TaskCompleted = 0;
            duplicateCount = 0;
            ExecuteLogCommand("Initializing download task...");

            string path = Properties.Settings.Default.Path;
            path += $"\\{repo.Subject.SyllabusCode} {(repo.Subject.Curriculum == Curriculums.ALevel ? "AL" : "GCSE")} {repo.Subject.Name}";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            await Task.Run(() =>
            {
                foreach (ExamYear year in repo)
                {
                    if (year.Specimen != null) DownloadPaper(year.Specimen, path);
                    if (year.Spring != null) DownloadPaper(year.Spring, path);
                    if (year.Summer != null) DownloadPaper(year.Summer, path);
                    if (year.Winter != null) DownloadPaper(year.Winter, path);
                }
            });
            ExecuteLogCommand($"{Tasks.Count} task{(Tasks.Count > 1 ? "s" : "")} enqueued{(duplicateCount == 0 ? "." : $", skipping {duplicateCount} duplicated file{(duplicateCount > 1 ? "s" : "")}.")}");
            ExecuteLogCommand($"Start downloading {subj.SyllabusCode} {subj.Name}");
            Message = "Downloading...";
            IsIndeterminate = false;

            await Task.Run(() =>
            {
                int part = Tasks.Count / 5;
                Task.Factory.StartNew(() =>
                {
                    TotalTasks = Tasks.Count;
                }, new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
                download(0, part, client1);
                download(part, part * 2, client2);
                download(part * 2, part * 3, client3);
                download(part * 3, part * 4, client4);
                download(part * 4, Tasks.Count, client5);
                while (TaskCompleted != Tasks.Count) { Thread.Sleep(500); }
            });
            ExecuteLogCommand($"Done. {Tasks.Count} items downloaded.");
            Message = "Done.";
            IsIndeterminate = true;
            Tasks.Clear();
        }

        private async void download(int start, int end, WebClient client)
        {
            await Task.Run(() =>
            {
                for (int i = start; i < end; i++)
                {
                    try
                    {
                        client.DownloadFile(Tasks[i].ResourceUrl, Tasks[i].LocalPath);
                    }
                    catch (Exception)
                    {
                        var index = i;
                        Task.Factory.StartNew(() =>
                        {
                            ExecuteLogCommand($"[ERROR] Failed to download {Tasks[index].FileName}.");
                        }, new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
                        continue;
                    }
                    finally
                    {
                        Task.Factory.StartNew(() =>
                        {
                            TaskCompleted += 1;
                        }, new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
                    }
                }
            });
        }

        WebClient client1 = new WebClient();
        WebClient client2 = new WebClient();
        WebClient client3 = new WebClient();
        WebClient client4 = new WebClient();
        WebClient client5 = new WebClient();

        int duplicateCount = 0;//TODO: thread sync
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
                        bool duplicate = false;
                        foreach (var f in MainWindowViewModel.Files)
                        {
                            if (file == f.Split('\\').Last())
                            {
                                duplicate = true;
                                duplicateCount++;
                                break;
                            }
                        }
                        if (!duplicate)
                        {
                            bool flag = false;
                            foreach (var task in Tasks)
                            {
                                if (task.LocalPath == $"{dir}\\{file}")
                                {
                                    duplicateCount++;
                                    flag = true;
                                }
                            }
                            if (flag) continue;

                            Task.Factory.StartNew(() => Tasks.Add(new DownloadTask
                            {
                                FileName = file,
                                State = DownloadTaskState.Pending,
                                Progress = 0,
                                ResourceUrl = paper.Url,
                                LocalPath = $"{dir}\\{file}",
                            }), new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
                        }
                    }
                }
            }

            if (exam.GradeThreshold is GradeThreshold)
            {
                Task.Factory.StartNew(() => Tasks.Add(new DownloadTask
                {
                    FileName = exam.GradeThreshold.Url.Split('/').Last(),
                    State = DownloadTaskState.Pending,
                    Progress = 0,
                    ResourceUrl = exam.GradeThreshold.Url,
                    LocalPath = $"{dir}\\{exam.GradeThreshold.Url.Split('/').Last()}",
                }), new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
            }
            if (exam.ExaminersReport is ExaminersReport)
            {
                Task.Factory.StartNew(() => Tasks.Add(new DownloadTask
                {
                    FileName = exam.ExaminersReport.Url.Split('/').Last(),
                    State = DownloadTaskState.Pending,
                    Progress = 0,
                    ResourceUrl = exam.ExaminersReport.Url,
                    LocalPath = $"{dir}\\{exam.ExaminersReport.Url.Split('/').Last()}",
                }), new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
            }
        }
    }
}
