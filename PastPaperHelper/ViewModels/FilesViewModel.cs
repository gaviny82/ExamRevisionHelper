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
        public static readonly Exam EmptyExam = new Exam();
        public FilesViewModel()
        {
            SelectedExamSeries = EmptyExam;
        }

        private Exam _selectedExamSeries;
        public Exam SelectedExamSeries
        {
            get { return _selectedExamSeries; }
            set { SetProperty(ref _selectedExamSeries, value); }
        }

        #region OpenExamSeriesCommand
        private DelegateCommand<Exam> _openExamSeriesCommand;
        public DelegateCommand<Exam> OpenExamSeriesCommand =>
            _openExamSeriesCommand ?? (_openExamSeriesCommand = new DelegateCommand<Exam>(ExecuteCommandName));

        void ExecuteCommandName(Exam exam)
        {
            SelectedExamSeries = exam;
        }
        #endregion

        #region OpenResourcesCommand
        private DelegateCommand<PastPaperResource> _openResourceCommand;
        public DelegateCommand<PastPaperResource> OpenResourcesCommand =>
            _openResourceCommand ?? (_openResourceCommand = new DelegateCommand<PastPaperResource>(ExecuteOpenResource));

        private void ExecuteOpenResource(PastPaperResource resource)
        {
            Process.Start(resource.State == ResourceStates.Offline && !string.IsNullOrEmpty(resource.Path) ?
            resource.Path :
            resource.Url);
        }
        #endregion
    }
}
