using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using ExamRevisionHelper.Core;
using ExamRevisionHelper.Core.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Prism.Mvvm;
using Prism.Regions;

namespace ExamRevisionHelper.ViewModels
{
    class PracticeViewModel : BindableBase, INavigationAware
    {
        public static ObservableCollection<MistakeViewModel> Mistakes = new ObservableCollection<MistakeViewModel>();
        public static Dictionary<Subject, IEnumerable<PracticeExamData>> MockExams = new Dictionary<Subject, IEnumerable<PracticeExamData>>();

        private static readonly string _mockExamDataPath = @$"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\PastPaperHelper\PastPaperHelper\mock_exams.xml";

        static PracticeViewModel()
        {
            if (!File.Exists(_mockExamDataPath)) return;

            XDocument doc = XDocument.Load(_mockExamDataPath);
            foreach (var node in doc.XPathSelectElements("/MockExams/Subject"))
            {
                var flag = PastPaperHelperCore.TryFindSubject(node.Attribute("SyllabusCode").Value, out Subject subj, App.CurrentInstance.SubjectsAvailable);
                if (!flag) continue;
                MockExams.Add(subj, from item in node.Elements("Exam")
                                    select new PracticeExamData
                                    {
                                        QuestionPaper = item.Attribute("QuestionPaper").Value,
                                        Date = DateTime.Parse(item.Attribute("Date").Value),
                                        TotalMarks = int.Parse(item.Attribute("TotalMarks").Value),
                                        Mark = int.Parse(item.Attribute("Mark").Value),
                                        Mistakes = (from q in item.Attribute("Mistakes").Value.Split(',') select int.Parse(q)).ToArray(),
                                    }
                    );
            }
        }

        public static void SaveMockExamsData()
        {
            XDocument doc = new XDocument(new XElement("MockExams",
                from subj in MockExams
                select new XElement("Subject",
                    new XAttribute("SyllabusCode", subj.Key.SyllabusCode),

                        from item in subj.Value
                        select new XElement("Exam",
    new XAttribute("QuestionPaper", item.QuestionPaper),
    new XAttribute("Date", item.Date),
    new XAttribute("TotalMarks", item.TotalMarks),
    new XAttribute("Mark", item.Mark),
    new XAttribute("Mistakes", string.Join(",", item.Mistakes))
))));
            doc.Save(_mockExamDataPath);
        }

        public PracticeViewModel()
        {
            RefreshCharts();
        }

        void RefreshCharts()
        {
            //Init models
            ExamSeriesCollection = new SeriesCollection();
            PieSeriesCollection = new SeriesCollection();
            int maxSize = 0;

            foreach (var item in MockExams)
            {
                //Refresh line chart
                var (subj, list) = (item.Key, item.Value);
                var series = new LineSeries
                {
                    Title = $"{subj.SyllabusCode} {subj.Name}",
                    Values = new ChartValues<double>(from data in list select 100D * (data.Mark / (double)data.TotalMarks))
                };
                if (series.Values.Count > maxSize) maxSize = series.Values.Count;
                ExamSeriesCollection.Add(series);
                foreach (var data in list)
                {
                    Mistakes.AddRange(from a in data.Mistakes select new MistakeViewModel { QuestionPaper = data.QuestionPaper, QuestionNumber = a });
                }
                int count = list.Count();
                TotalNumberOfPapers += count;

                //Refresh pie chart
                PieSeriesCollection.Add(new PieSeries
                {
                    Title = $"{subj.SyllabusCode} {subj.Name}",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(count) },
                    DataLabels = true
                });
            }
            Labels = new string[maxSize];
            for (int i = 0; i < maxSize; i++)
            {
                Labels[i] = (i + 1).ToString();
            }
        }

        #region Implement INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RefreshCharts();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
        #endregion

        private int _totalNumberOfPapers = 0;
        public int TotalNumberOfPapers
        {
            get { return _totalNumberOfPapers; }
            set { SetProperty(ref _totalNumberOfPapers, value); }
        }

        public SeriesCollection PieSeriesCollection { get; set; }

        public SeriesCollection ExamSeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; } = (value) => string.Format("{0:F2}%", value);
    }

    class MistakeViewModel : BindableBase
    {
        public string QuestionPaper { get; set; }
        public int QuestionNumber { get; set; }
    }

    struct PracticeExamData
    {
        public int TotalMarks { get; set; }
        public int Mark { get; set; }
        public DateTime Date { get; set; }
        public string QuestionPaper { get; set; }
        public int[] Mistakes { get; set; }
    }
}