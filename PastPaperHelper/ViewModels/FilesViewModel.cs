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
        public FilesViewModel()
        {
            ExamSeries = "-";
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
}
