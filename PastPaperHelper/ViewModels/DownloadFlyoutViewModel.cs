﻿using PastPaperHelper.Core.Tools;
using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using PastPaperHelper.Views;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PastPaperHelper.ViewModels
{
    public class DownloadFlyoutViewModel : BindableBase
    {
        public static ObservableCollection<DownloadTask> Tasks = new ObservableCollection<DownloadTask>();
        public static readonly object DownloadTasksListLock = new object();
        public static CancellationTokenSource DownloadTaskCancellation = new CancellationTokenSource();
        private static CancellationToken _cancelationToken = DownloadTaskCancellation.Token;
        private static bool _isDownloading = false;
        private static int _taskFailedCounter = 0;

        public static Task[] DownloadTaskPool;
        private static WebClient[] _webClientPool;
        private static int duplicateCount = 0;

        public bool IsDownloading
        {
            get { return _isDownloading; }
            set { SetProperty(ref _isDownloading, value); }
        }

        #region DownloadCommand
        private DelegateCommand<IEnumerable<DownloadTask>> _downloadCommand;
        public DelegateCommand<IEnumerable<DownloadTask>> DownloadCommand =>
            _downloadCommand ?? (_downloadCommand = new DelegateCommand<IEnumerable<DownloadTask>>(ExecuteDownloadCommand));

        async void ExecuteDownloadCommand(IEnumerable<DownloadTask> tasks)
        {
            int additional = 0;
            var fileslst = (from task in Tasks select task.FileName).ToList();
            var pendinglst = tasks.ToList();

            await Task.Run(() =>
            {
                lock (DownloadTasksListLock)
                {
                    duplicateCount = 0;
                    foreach (DownloadTask item in pendinglst)
                    {
                        if (!fileslst.Contains(item.FileName))
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Tasks.Add(item);
                            });
                            fileslst.Add(item.FileName);
                            additional++;
                        }
                        else duplicateCount++;
                    }
                }
            });

            TotalTasks += additional;
            ExecuteLogCommand($"{pendinglst.Count - duplicateCount} task{(Tasks.Count > 1 ? "s" : "")} enqueued{(duplicateCount == 0 ? "." : $", skipping {duplicateCount} duplicated file{(duplicateCount > 1 ? "s" : "")}.")}");

            if (IsDownloading) return;
            InitDownloading();
        }

        #endregion

        #region LogCommand
        private DelegateCommand<string> _logCommand;
        public DelegateCommand<string> LogCommand =>
            _logCommand ?? (_logCommand = new DelegateCommand<string>(ExecuteLogCommand));

        void ExecuteLogCommand(string param)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Log += $"\n[{DateTime.Now.ToShortTimeString()}] {param.ToString()}";
            });
        }
        #endregion

        #region RetryCommand
        private DelegateCommand _retryCommand;
        public DelegateCommand RetryCommand =>
            _retryCommand ?? (_retryCommand = new DelegateCommand(ExecuteRetryCommand));

        async void ExecuteRetryCommand()
        {
            int retryCount = 0;
            await Task.Run(() =>
            {
                lock (DownloadTasksListLock)
                {
                    foreach (var item in Tasks)
                    {
                        if (item.State == DownloadTaskState.Error)
                        {
                            item.State = DownloadTaskState.Pending;
                            retryCount++;
                        }
                    }
                }
            });

            ExecuteLogCommand($"Retrying {retryCount} item{(retryCount > 1 ? "" : "s")}...");
            if (IsDownloading) return;
            InitDownloading();
        }
        #endregion

        private void InitDownloading()
        {
            Message = "Downloading...";
            IsDownloading = true;
            IsIndeterminate = false;

            TaskFactory factory = new TaskFactory();
            DownloadTaskPool = new Task[10];
            _webClientPool = new WebClient[10];
            for (int i = 0; i < 10; i++)
            {
                var client = new WebClient();
                _webClientPool[i] = client;
                DownloadTaskPool[i] = factory.StartNew(
                    action: DownloadLoop,
                    state: client,
                    cancellationToken: _cancelationToken);
            }

            factory.ContinueWhenAll(DownloadTaskPool, (results) =>
            {
                DownloadTaskCancellation.Dispose();
                DownloadTaskCancellation = new CancellationTokenSource();
                _cancelationToken = DownloadTaskCancellation.Token;
                if (Tasks.Count != 0) ExecuteLogCommand($"[ERROR]. Failed to download {Tasks.Count} items, please retry.");
                ExecuteLogCommand($"Done. {TotalTasks} items downloaded.");
                _taskFailedCounter = 0;
                IsDownloading = false;
                TaskCompleted = 0;
                TotalTasks = 0;
                Message = "Done.";
                IsIndeterminate = true;
            });
        }

        private void DownloadLoop(object param)
        {
            WebClient client = param as WebClient;
            _cancelationToken.ThrowIfCancellationRequested();
            while (true)
            {
                if (_cancelationToken.IsCancellationRequested)
                {
                    _cancelationToken.ThrowIfCancellationRequested();
                }

                DownloadTask task = null;
                int currentIndex = -1;
                lock (DownloadTasksListLock)
                {
                    for (int i = _taskFailedCounter; i < Tasks.Count; i++)
                    {
                        if (Tasks[i].State == DownloadTaskState.Pending)
                        {
                            task = Tasks[i];
                            currentIndex = i;
                            break;
                        }
                    }
                    if (currentIndex == -1) break;
                    task.State = DownloadTaskState.Downloading;
                }

                if (File.Exists(task.LocalPath))
                {
                    task.State = DownloadTaskState.Error;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        TaskCompleted++;
                    });
                    ExecuteLogCommand($"Failed to download {task.FileName}. File already exists at {task.LocalPath}");
                }
                else
                {
                    //Random random = new Random();random.
                    //var n = random.Next(0, 2); Console.WriteLine(n);
                    //if (n == 1)
                    //{
                    //    ExecuteLogCommand($"Failed to download {task.FileName}. File already exists at {task.LocalPath}"); task.State = DownloadTaskState.Error;  continue; }
                    Thread.Sleep(750);
                    task.State = DownloadTaskState.Completed;
                    lock (DownloadTasksListLock)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Tasks.Remove(task);
                            TaskCompleted++;
                        });
                    }
                }
            }

        }

        #region PauseDownloadCommand
        private DelegateCommand _pauseDownloadCommand;
        public DelegateCommand PauseDownloadCommand =>
            _pauseDownloadCommand ?? (_pauseDownloadCommand = new DelegateCommand(ExecutePauseDownloadCommand));

        void ExecutePauseDownloadCommand()
        {

        }
        #endregion

        #region CancelDownloadCommand
        private DelegateCommand _cancelDownloadCommand;
        public DelegateCommand CancelDownloadCommand =>
            _cancelDownloadCommand ?? (_cancelDownloadCommand = new DelegateCommand(ExecuteCancelDownloadCommand));

        void ExecuteCancelDownloadCommand()
        {
            DownloadTaskCancellation.Cancel();
        }
        #endregion


        #region Property: TotalTasks
        private int _totalTasks;
        public int TotalTasks
        {
            get { return _totalTasks; }
            set { SetProperty(ref _totalTasks, value); }
        }
        #endregion

        #region Property: TaskCompletd
        private int _taskCompleted;
        public int TaskCompleted
        {
            get { return _taskCompleted; }
            set { SetProperty(ref _taskCompleted, value); }
        }
        #endregion

        #region Property: IsIndeterminate
        private bool _isIndeterminate = true;
        public bool IsIndeterminate
        {
            get { return _isIndeterminate; }
            set { SetProperty(ref _isIndeterminate, value); }
        }
        #endregion

        #region Property: Log
        private string _log = $"[{DateTime.Now.ToShortTimeString()}] Download service initialized.";
        public string Log
        {
            get { return _log; }
            set { SetProperty(ref _log, value); }
        }
        #endregion

        #region Property: Message
        private string _message = "Waiting for download...";
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
        #endregion

        public DownloadFlyoutViewModel()
        {
            
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
    }
}
