using PastPaperHelper.Models;
using PastPaperHelper.ViewModels;
using System.Diagnostics;
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
        }

        private void SubjectSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as FilesViewModel).SelectedExamSeries = new Exam();
        }

        private void ViewPaper_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(((sender as Button).DataContext as Paper).Url);
        }
    }
}
