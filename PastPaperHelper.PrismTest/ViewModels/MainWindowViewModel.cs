using Prism.Commands;
using Prism.Mvvm;

namespace PastPaperHelper.PrismTest.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "PastPaperHelper";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
