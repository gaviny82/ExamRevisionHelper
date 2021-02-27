using System.Windows;
using System.Windows.Controls;
using ExamRevisionHelper.Core;
using ExamRevisionHelper.Core.Models;
using ExamRevisionHelper.ViewModels;

namespace ExamRevisionHelper.Views
{
    /// <summary>
    /// Interaction logic for FilesView
    /// </summary>
    public partial class FilesView : UserControl
    {
        public FilesView()
        {
            InitializeComponent();
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

        private void SubjectSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as FilesViewModel).SelectedExamSeries = FilesViewModel.EmptyExam;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as FilesViewModel).OpenPaperCommand.Execute((sender as MenuItem).DataContext as Variant);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            (App.Current.MainWindow.DataContext as MainWindowViewModel)
                .StartMockExamCommand.Execute((sender as MenuItem).DataContext as Variant);
        }
    }
}
