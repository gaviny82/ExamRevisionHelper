using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExamRevisionHelper.Core;
using ExamRevisionHelper.Core.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace ExamRevisionHelper.ViewModels
{
    public class DownloadViewModel : BindableBase
    {
        public DownloadFlyoutViewModel DownloadFlyoutViewModel;

        public DownloadViewModel()
        {

        }


        private DelegateCommand<Subject> _downloadCommand;
        public DelegateCommand<Subject> DownloadCommand =>
            _downloadCommand ?? (_downloadCommand = new DelegateCommand<Subject>(ExecuteDownloadCommandAsync));

        async void ExecuteDownloadCommandAsync(Subject subj)
        {
            if (subj == null) return;

            DownloadFlyoutViewModel.LogCommand.Execute($"Initializing download tasks for {subj.SyllabusCode} {subj.Name}");
            DownloadFlyoutViewModel.IsIndeterminate = false;

            PaperRepository repo = PastPaperHelperCore.Source.Subscription[subj];
            string path = PastPaperHelperCore.LocalFilesPath;
            path += $"\\{repo.Subject.SyllabusCode} {(repo.Subject.Curriculum == Curriculums.ALevel ? "AL" : "GCSE")} {repo.Subject.Name}";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            List<DownloadTask> tasks = new List<DownloadTask>();
            await Task.Run(() =>
            {
                foreach (ExamYear year in repo)
                {
                    if (year.Specimen != null) tasks.AddRange(GetDownloadTasks(year.Specimen, path));
                    if (year.Spring != null) tasks.AddRange(GetDownloadTasks(year.Spring, path));
                    if (year.Summer != null) tasks.AddRange(GetDownloadTasks(year.Summer, path));
                    if (year.Winter != null) tasks.AddRange(GetDownloadTasks(year.Winter, path));
                }
            });
            DownloadFlyoutViewModel.DownloadCommand.Execute(tasks);
        }
        private static IEnumerable<DownloadTask> GetDownloadTasks(Exam exam, string dir)
        {
            string series = exam.Series switch
            {
                ExamSeries.Spring => "March",
                ExamSeries.Summer => "May-June",
                ExamSeries.Winter => "Oct-Nov",
                ExamSeries.Specimen => "Specimen",
                _ => throw new Exception($"ExamSeries name error {exam.Series.ToString()}."),
            };

            dir += $"\\{exam.Year} {series}";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            List<DownloadTask> tasks = new List<DownloadTask>();
            Action<PastPaperResource> tryAddToTasks = (item) =>
            {
                string file = item.Url.Split('/').Last();
                if (!PastPaperHelperCore.LocalFiles.Keys.Contains(file))
                {
                    tasks.Add(new DownloadTask
                    {
                        FileName = file,
                        State = DownloadTaskState.Pending,
                        Progress = 0,
                        ResourceUrl = item.Url,
                        LocalPath = $"{dir}\\{file}",
                    });
                }
            };
            var lst = (from comp in exam.Components select comp.Variants);
            foreach (var varients in lst)
            {
                foreach (var item in varients)
                {
                    foreach (Paper paper in item.Papers)
                    {
                        tryAddToTasks(paper);
                    }
                }
            }

            if (exam.GradeThreshold is GradeThreshold) tryAddToTasks(exam.GradeThreshold);
            if (exam.ExaminersReport is ExaminersReport) tryAddToTasks(exam.ExaminersReport);

            return tasks;
        }

    }
}
