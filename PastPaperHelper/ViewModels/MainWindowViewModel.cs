using PastPaperHelper.Models;
using PastPaperHelper.Events;
using PastPaperHelper.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;

namespace PastPaperHelper.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public static ObservableCollection<Subject> SubscribedSubjects { get; private set; } = new ObservableCollection<Subject>();

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

    }
}
