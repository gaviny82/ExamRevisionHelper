using PastPaperHelper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.ViewModels
{
    public class FilesViewModel : NotificationObject
    {
        private ObservableCollection<ExamYearViewModel> _examYears = new ObservableCollection<ExamYearViewModel>();
        public IEnumerable<ExamYearViewModel> ExamYears
        {
            get { return _examYears; }
        }
        public FilesViewModel()
        {
            ExamSeries = "-";
            _examYears.Add(new ExamYearViewModel { Year = "2018" });
        }

        private string _examSeries;
        public string ExamSeries
        {
            get { return _examSeries; }
            set { _examSeries = value; RaisePropertyChangedEvent("ExamSeries"); }
        }

        private Subject _selectedSubject;
        public Subject SelectedSubject
        {
            get { return _selectedSubject; }
            set { _selectedSubject = value; RaisePropertyChangedEvent("SelectedSubject"); }
        }


    }

    public class ExamYearViewModel : NotificationObject
    {
        public string Year { get; set; }
        public ExamSeriesViewModel Spring { get; set; }
        public ExamSeriesViewModel Summer { get; set; }
        public ExamSeriesViewModel Winter { get; set; }

    }

    public class ExamSeriesViewModel : NotificationObject
    {
        public string Year { get; set; }
        public ExamSeries ExamSeries { get; set; }
        public Subject Subject { get; set; }
        public override string ToString()
        {
            string str = "";
            switch (ExamSeries)
            {
                case ExamSeries.Spring:
                    str = "March";
                    break;
                case ExamSeries.Summer:
                    str = "May/June";
                    break;
                case ExamSeries.Winter:
                    str = "Oct/Nov";
                    break;
            }
            return Year + " " + str;
        }
    }
}
