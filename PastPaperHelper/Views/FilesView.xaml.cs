using PastPaperHelper.Models;
using PastPaperHelper.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Net;
using PastPaperHelper.Sources;
using System.Collections.Generic;
using System.Windows;
using System.Threading;

namespace PastPaperHelper.Views
{
    /// <summary>
    /// FilesView.xaml 的交互逻辑
    /// </summary>
    public partial class FilesView : Grid
    {
        public FilesView()
        {
            InitializeComponent();
            DataContext = new FilesViewModel();
        }

        private void SubjectSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as FilesViewModel).SelectedExamSeries = new Exam();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox view = sender as ListBox;
            if (view.SelectedItem != null)
            {
                (DataContext as FilesViewModel).OpenOnlineResourceCommand.Execute(view.SelectedItem);
            }
        }

        private void ViewPaper_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(((sender as Button).DataContext as Paper).Url);
        }

        private void Download_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PaperRepository repo = SubscriptionManager.Subscription[(Subject)subjectSelector.SelectedItem];
            progress.Value = 0;
            error.Text = "";
            Task.Run(() =>
            {
                if (failedTasks.Count != 0)
                {
                    downloadTasks.AddRange(failedTasks);
                    failedTasks.Clear();
                }
                else
                {
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
                }

                int part = downloadTasks.Count / 5;
                Task.Factory.StartNew(() =>
                {
                    progress.Maximum = downloadTasks.Count;
                }, new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
                download(0, part, client1);
                download(part, part * 2, client2);
                download(part * 2, part * 3, client3);
                download(part * 3, part * 4, client4);
                download(part * 4, downloadTasks.Count, client5);
            });
        }
        public void download(int start, int end, WebClient client)
        {
            Task.Run(() =>
            {
                for (int i = start; i < end; i++)
                {
                    try
                    {
                        client.DownloadFile(downloadTasks[i].Item1, downloadTasks[i].Item2);
                    }
                    catch (System.Exception)
                    {
                        var index = i;
                        failedTasks.Add(downloadTasks[index]);
                        Task.Factory.StartNew(() =>
                        {
                            error.Text += "\n" + downloadTasks[index].Item1;
                        }, new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
                        continue;
                    }
                    finally
                    {
                        Task.Factory.StartNew(() =>
                        {
                            progress.Value += 1;
                        }, new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
                    }
                }
            });
        }

        List<(string, string)> downloadTasks = new List<(string, string)>();
        List<(string, string)> failedTasks = new List<(string, string)>();
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
                        downloadTasks.Add((paper.Url, dir + $"\\{paper.Url.Split('/').Last()}"));
                    }
                }
            }
        }
    }
}
