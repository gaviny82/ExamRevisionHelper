using PastPaperHelper.ViewModels;
using System.Windows.Controls;

namespace PastPaperHelper.Views
{
    /// <summary>
    /// Interaction logic for DownloadView
    /// </summary>
    public partial class DownloadView : UserControl
    {
        public DownloadView()
        {
            InitializeComponent();
            (DataContext as DownloadViewModel).DownloadFlyoutViewModel = downloaPanel.DataContext as DownloadFlyoutViewModel;
        }
    }
}
