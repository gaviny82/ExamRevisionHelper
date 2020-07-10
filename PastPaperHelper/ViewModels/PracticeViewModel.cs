using PastPaperHelper.Models;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PastPaperHelper.ViewModels
{
    public class PracticeViewModel : BindableBase, INavigationAware
    {
        public static Variant MockPaper;

        private bool _isMockStarted = false;
        public bool IsMockStarted
        {
            get { return _isMockStarted; }
            set { SetProperty(ref _isMockStarted, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var param = navigationContext.Parameters;
            if (param.ContainsKey("MockPaper"))
            {
                IsMockStarted = true;
                Message = "When your are ready for the exam, click the button below to start the timer.";
                MockPaper = navigationContext.Parameters["MockPaper"] as Variant;
                //TODO: start mock exam
            }
        }
    }
}
