using PastPaperHelper.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PastPaperHelper.ViewModels
{
    public class FilesViewModel : BindableBase
    {
        public FilesViewModel()
        {
            SelectedExamSeries = new Exam();
        }

        private Exam _selectedExamSeries;
        public Exam SelectedExamSeries
        {
            get { return _selectedExamSeries; }
            set { SetProperty(ref _selectedExamSeries, value); }
        }

        private DelegateCommand<Exam> _openExamSeriesCommand;
        public DelegateCommand<Exam> OpenExamSeriesCommand =>
            _openExamSeriesCommand ?? (_openExamSeriesCommand = new DelegateCommand<Exam>(ExecuteCommandName));

        void ExecuteCommandName(Exam exam)
        {
            SelectedExamSeries = exam;
        }

        private DelegateCommand<PastPaperResource> _openOnlineResourceCommand;
        public DelegateCommand<PastPaperResource> OpenOnlineResourcesCommand =>
            _openOnlineResourceCommand ?? (_openOnlineResourceCommand = new DelegateCommand<PastPaperResource>(OpenOnlineResource));

        private void OpenOnlineResource(PastPaperResource resource)
        {
            Process.Start(resource.Url);
        }
    }
}
