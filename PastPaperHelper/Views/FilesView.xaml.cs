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
            Task.Run(() =>
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
            });
        }
        WebClient client = new WebClient();
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
                        client.DownloadFile(paper.Url, dir + $"\\{paper.Url.Split('/').Last()}");
                    }
                }
            }
        }
    }
}
