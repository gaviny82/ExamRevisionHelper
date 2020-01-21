using PastPaperHelper.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.ViewModels
{
    public class DownloadFlyoutViewModel : NotificationObject
    {
        public DownloadFlyoutViewModel()
        {
            MainWindow.DownloadFlyoutViewModel = this;
        }
    }
}
