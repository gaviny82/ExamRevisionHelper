using PastPaperHelper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.ViewModels
{
    public class FilesViewModel : NotificationObject
    {
        public FilesViewModel()
        {
            OpenExamSeriesCommand = new DelegateCommand(OpenExamSeries);
            OpenOnlineResourceCommand = new DelegateCommand(OpenOnlineResource);
        }


        private Exam _selectedExamSeries;
        public Exam SelectedExamSeries
        {
            get { return _selectedExamSeries; }
            set { _selectedExamSeries = value; RaisePropertyChangedEvent("SelectedExamSeries"); }
        }

        public DelegateCommand OpenExamSeriesCommand { get; set; }
        private void OpenExamSeries(object param)
        {
            SelectedExamSeries = param as Exam;
        }


        public DelegateCommand OpenOnlineResourceCommand { get; set; }
        private void OpenOnlineResource(object param)
        {
            PastPaperResource res = param as PastPaperResource;
            if (res != null) Process.Start(res.Url);
        }
    }
}
