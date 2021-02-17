using System.Windows.Controls;
using ExamRevisionHelper.ViewModels;

namespace ExamRevisionHelper.Views
{
    /// <summary>
    /// DownloadFlyout.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadFlyout : UserControl
    {
        public DownloadFlyout()
        {
            InitializeComponent();
            DataContext = new DownloadFlyoutViewModel();
        }
    }
}
