using ExamRevisionHelper.Models;
using Prism.Commands;
using Prism.Windows.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.System;

namespace ExamRevisionHelper.ViewModels
{
    public class PapersPageViewModel : ViewModelBase
    {


        private Exam _selectedExamSeries;
        public Exam SelectedExamSeries
        {
            get { return _selectedExamSeries; }
            set { SetProperty(ref _selectedExamSeries, value); }
        }

        public ObservableCollection<ComponentViewModel> Components = new ObservableCollection<ComponentViewModel>();

        private DelegateCommand<Exam> _openExamSeriesCommand;
        public DelegateCommand<Exam> OpenExamSeriesCommand =>
            _openExamSeriesCommand ?? (_openExamSeriesCommand = new DelegateCommand<Exam>(ExecuteOpenExamSeriesCommand));

        bool flag = false;

        void ExecuteOpenExamSeriesCommand(Exam parameter)
        {
            flag = !flag;
            SelectedExamSeries = parameter;
            Components.Clear();
            if (flag) return;
            foreach (Component item in SelectedExamSeries.Components)
            {
                var vm = new ComponentViewModel(item.Variants)
                {
                    Data = item,
                    Code=item.Code+"",
                };
                Components.Add(vm);
            }
        }

        private DelegateCommand<Paper> _openPaperCommand;
        public DelegateCommand<Paper> OpenPaperCommand =>
            _openPaperCommand ?? (_openPaperCommand = new DelegateCommand<Paper>(ExecuteOpenPaperCommand));

        void ExecuteOpenPaperCommand(Paper parameter)
        {
            _ = Launcher.LaunchUriAsync(new System.Uri(parameter.Url));
        }
    }

    public class ComponentViewModel : ViewModelBase
    {
        public Component Data { get; set; }

        private string _code;

        public ComponentViewModel()
        {

        }
        public ComponentViewModel(Variant[] variants)
        {
            foreach (Variant item in variants)
            {
                VariantViewModel model = new VariantViewModel();
                model.Papers = new ObservableCollection<Paper>(item.Papers);
                model.VariantCode = item.VariantCode+"";
                model.Data = item;
                Variants.Add(model);
            }
        }

        public string Code
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }

        public ObservableCollection<VariantViewModel> Variants { get; set; } = new ObservableCollection<VariantViewModel>();
    }

    public class VariantViewModel : ViewModelBase
    {
        private string _variantCode;
        public string VariantCode
        {
            get { return _variantCode; }
            set { SetProperty(ref _variantCode, value); }
        }
        public Variant Data { get; set; }

        public ObservableCollection<Paper> Papers { get; set; } = new ObservableCollection<Paper>();
    }
}
