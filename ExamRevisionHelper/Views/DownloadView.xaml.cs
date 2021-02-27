using System.Windows;
using System.Windows.Controls;
using ExamRevisionHelper.Core;
using ExamRevisionHelper.ViewModels;

namespace ExamRevisionHelper.Views
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

            App.CurrentInstance.Updater.UpdateServiceNotifiedEvent += (args) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (args.NotificationType == NotificationType.Finished)
                    {
                        subjectSelector.SelectedIndex = 0;
                    }
                });
            };

            App.CurrentInstance.Updater.SubjectSubscribedEvent += (args) =>
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
