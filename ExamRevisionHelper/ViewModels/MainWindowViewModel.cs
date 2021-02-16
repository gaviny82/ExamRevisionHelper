using ExamRevisionHelper.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using ExamRevisionHelper.Core.Tools;

namespace ExamRevisionHelper.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public static ObservableCollection<Subject> SubscribedSubjects { get; private set; } = new ObservableCollection<Subject>();

        public static void RefreshSubscribedSubjects()
        {
            SubscribedSubjects.Clear();
            foreach (Subject item in PastPaperHelperCore.SubscribedSubjects)
            {
                SubscribedSubjects.Add(item);
            }
        }

        private readonly IRegionManager _regionManager;

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        #region NavigateCommand
        private DelegateCommand<string> _navigateCommand;
        public DelegateCommand<string> NavigateCommand =>
            _navigateCommand ?? (_navigateCommand = new DelegateCommand<string>(Navigate));
        private void Navigate(string uri)
        {
            PageTitle = uri;
            _regionManager.RequestNavigate("ContentRegion", uri.Replace(" ", ""));
        }
        #endregion

        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set { SetProperty(ref _pageTitle, value); }
        }

        private DelegateCommand<Variant> _startMockExam;
        public DelegateCommand<Variant> StartMockExamCommand =>
            _startMockExam ?? (_startMockExam = new DelegateCommand<Variant>(ExecuteStartMockExamCommand));

        void ExecuteStartMockExamCommand(Variant parameter)
        {
            PageTitle = "Mock exam";
            NavigationParameters navParam = new NavigationParameters
            {
                { "MockPaper", parameter }
            };
            NavigationSelectionIndex = -1;
            _regionManager.RequestNavigate("ContentRegion", "Countdown", navParam);
        }

        private int _navigationSelectionIndex = 0;
        public int NavigationSelectionIndex
        {
            get { return _navigationSelectionIndex; }
            set { SetProperty(ref _navigationSelectionIndex, value); }
        }

        private DelegateCommand<Variant> _finishMockExam;
        public DelegateCommand<Variant> FinishMockExamCommand =>
            _finishMockExam ?? (_finishMockExam = new DelegateCommand<Variant>(ExecuteFinishMockExamCommand));

        void ExecuteFinishMockExamCommand(Variant parameter)
        {
            PageTitle = "Marking";
            NavigationParameters navParam = new NavigationParameters
            {
                { "MockPaper", parameter }
            };
            NavigationSelectionIndex = -1;
            _regionManager.RequestNavigate("ContentRegion", "MarkPaper", navParam);
        }
    }
}
