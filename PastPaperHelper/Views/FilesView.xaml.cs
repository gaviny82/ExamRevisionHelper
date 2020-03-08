using PastPaperHelper.Core.Tools;
using PastPaperHelper.Models;
using PastPaperHelper.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PastPaperHelper.Views
{
    /// <summary>
    /// Interaction logic for FilesView
    /// </summary>
    public partial class FilesView : UserControl
    {
        public FilesView()
        {
            InitializeComponent();
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

        private void SubjectSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as FilesViewModel).SelectedExamSeries = FilesViewModel.EmptyExam;
        }
    }
}
