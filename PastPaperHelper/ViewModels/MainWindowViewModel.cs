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
        private readonly IRegionManager _regionManager;

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            //eventAggregator.GetEvent<MessageBarEnqueuedEvent>().Subscribe((msg) =>
            //{
            //    MainWindow.MainSnackbar.MessageQueue.Enqueue(msg);
            //}, ThreadOption.UIThread);
        }

        private DelegateCommand<string> _navigateCommand;
        public DelegateCommand<string> NavigateCommand =>
            _navigateCommand ?? (_navigateCommand = new DelegateCommand<string>(Navigate));
        private void Navigate(string uri)
        {
            Title = uri;
            _regionManager.RequestNavigate("ContentRegion", uri.Replace(" ", ""));
        }


        private string _title = "PastPaperHelper";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

    }

    public static class ApplicationCommands
    {
        
    }
}
