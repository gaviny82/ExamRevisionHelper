using PastPaperHelper.PrismTest.Commands;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace PastPaperHelper.PrismTest.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }


        private DelegateCommand<string> _navigateCommand;
        public DelegateCommand<string> NavigateCommand =>
            _navigateCommand ?? (_navigateCommand = new DelegateCommand<string>(Navigate));
        private void Navigate(string uri)
        {
            Title = uri;
            _regionManager.RequestNavigate("ContentRegion", uri);
        }


        private string _title = "PastPaperHelper";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
    }
}
