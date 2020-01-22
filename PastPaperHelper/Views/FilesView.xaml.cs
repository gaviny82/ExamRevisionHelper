using PastPaperHelper.Models;
using PastPaperHelper.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Net;
using PastPaperHelper.Sources;
using System.Collections.Generic;
using System.Windows;
using System.Threading;

namespace PastPaperHelper.Views
{
    /// <summary>
    /// FilesView.xaml 的交互逻辑
    /// </summary>
    public partial class FilesView : Grid
    {
        public FilesView()
        {
            InitializeComponent();
            DataContext = new FilesViewModel();
        }

        private void SubjectSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as FilesViewModel).SelectedExamSeries = new Exam();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox view = sender as ListBox;
            if (view.SelectedItem != null)
            {
                (DataContext as FilesViewModel).OpenOnlineResourceCommand.Execute(view.SelectedItem);
            }
        }

        private void ViewPaper_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(((sender as Button).DataContext as Paper).Url);
        }

        private void Download_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MainWindow.DownloadFlyoutViewModel.DownloadCommand.Execute((Subject)subjectSelector.SelectedItem);
            (Application.Current.MainWindow as MainWindow).OpenDownloadPopup();
        }
    }
}
