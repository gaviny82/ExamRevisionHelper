using PastPaperHelper.Models;
using PastPaperHelper.ViewModels;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace PastPaperHelper.Views
{
    /// <summary>
    /// FilesView.xaml 的交互逻辑
    /// </summary>
    public partial class FilesView : Page
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
    }
}
