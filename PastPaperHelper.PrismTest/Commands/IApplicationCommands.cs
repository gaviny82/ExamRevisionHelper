using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.PrismTest.Commands
{
    public interface IApplicationCommands
    {
        DelegateCommand<string> NavigateDialogCommand { get; }
    }
}
