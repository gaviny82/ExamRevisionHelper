using PastPaperHelper.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Xml.XPath;

namespace PastPaperHelper.ViewModels
{
    class PracticeViewModel : BindableBase
    {
        public static ObservableCollection<PracticeExamData> MockExams = new ObservableCollection<PracticeExamData>();

        private static readonly string _mockExamDataPath = @$"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\PastPaperHelper\PastPaperHelper\mock_exams.xml";
        
        static PracticeViewModel()
        {
            if (!File.Exists(_mockExamDataPath)) return;

            XDocument doc = XDocument.Load(_mockExamDataPath);
            var list = from item in doc.XPathSelectElements("/MockExams//Exam")
                       select new PracticeExamData
                       {
                           QuestionPaper = item.Attribute("QuestionPaper").Value,
                           Date = DateTime.Parse(item.Attribute("Date").Value),
                           TotalMarks = int.Parse(item.Attribute("TotalMarks").Value),
                           Mark = int.Parse(item.Attribute("Mark").Value),
                           Mistakes = (from q in item.Attribute("Mistakes").Value.Split(',') select int.Parse(q)).ToArray(),
                       };
            MockExams.AddRange(list);
        }

        public static void SaveMockExamsData()
        {
            XDocument doc = new XDocument(new XElement("MockExams",
                from item in MockExams
                select new XElement("Exam",
                    new XAttribute("QuestionPaper", item.QuestionPaper),
                    new XAttribute("Date", item.Date),
                    new XAttribute("TotalMarks", item.TotalMarks),
                    new XAttribute("Mark", item.Mark),
                    new XAttribute("Mistakes", string.Join(",", item.Mistakes))
                )));
            doc.Save(_mockExamDataPath);
        }
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
