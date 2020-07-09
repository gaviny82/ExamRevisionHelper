using PastPaperHelper.Core.Tools;
using PastPaperHelper.ViewModels;
using System.Windows;
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
            (DataContext as DownloadViewModel).DownloadFlyoutViewModel = downloadPanel.DataContext as DownloadFlyoutViewModel;

            PastPaperHelperUpdateService.UpdateServiceNotifiedEvent += (args) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (args.NotificationType == NotificationType.Finished)
                    {
                        subjectSelector.SelectedIndex = 0;
                    }
                });
            };

            PastPaperHelperUpdateService.SubjectSubscribedEvent += (args) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (subjectSelector.SelectedIndex == -1)
                        subjectSelector.SelectedIndex = 0;
                });
            };
        }
    }
}
