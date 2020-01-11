using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.PrismTest.Commands
{
    class ApplicationCommands : IApplicationCommands
    {
        private readonly IRegionManager _regionManager;

        public ApplicationCommands(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        private DelegateCommand<string> _navigateDialogCommand;
        public DelegateCommand<string> NavigateDialogCommand =>
            _navigateDialogCommand ?? (_navigateDialogCommand = new DelegateCommand<string>(ExecuteNavigateDialogCommand));

        void ExecuteNavigateDialogCommand(string uri)
        {
            _regionManager.RequestNavigate("Dialog", uri);
        }
    }
}
