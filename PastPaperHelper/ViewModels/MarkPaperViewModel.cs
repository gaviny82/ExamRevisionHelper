using PastPaperHelper.Models;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.ViewModels
{
    public class MarkPaperViewModel : BindableBase, INavigationAware
    {
        public Variant MockPaper;

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var param = navigationContext.Parameters;
            if (param.ContainsKey("MockPaper"))
            {
                MockPaper = param["MockPaper"] as Variant;
            }
        }
    }
}
