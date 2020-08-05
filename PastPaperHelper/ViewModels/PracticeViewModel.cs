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
        public static XDocument MockExamsData;

        static PracticeViewModel()
        {
            var mockExamDataPath = @$"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\PastPaperHelper\PastPaperHelper\mock_exams.xml";
            if (File.Exists(mockExamDataPath))
            {
                MockExamsData = XDocument.Load(mockExamDataPath);
            }
            else
            {
                MockExamsData = new XDocument(new XElement("MockExams"));
                MockExamsData.Save(mockExamDataPath);
            }

            var list = from item in MockExamsData.XPathSelectElements("/MockExams//Exam")
                       select new PracticeExamData
                       {
                           Date = DateTime.Parse(item.Attribute("Date").Value),
                           TotalMarks = int.Parse(item.Attribute("TotalMarks").Value),
                           Mark = int.Parse(item.Attribute("Mark").Value),
                           Mistakes = (from q in item.Attribute("Mistakes").Value.Split(',') select int.Parse(q)).ToArray(),
                       };
            MockExams.AddRange(list);
        }

        public static void SaveMockExamsData()
        {

        }
    }

    struct PracticeExamData
    {
        public int TotalMarks { get; set; }
        public int Mark { get; set; }
        public DateTime Date { get; set; }
        public Variant ExamPaper { get; set; }
        public int[] Mistakes { get; set; }
    }
}
