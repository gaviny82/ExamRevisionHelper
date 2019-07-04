using MaterialDesignThemes.Wpf;
using PastPaperHelper.Models;

namespace PastPaperHelper
{
    /// <summary>
    /// PaperDownloadView.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadView : DialogHost
    {
        public DownloadView()
        {
            InitializeComponent();
        }

        private void RootDlg_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            double newWidth = e.NewSize.Width, newHeight = e.NewSize.Height;
            dlgGrid.Width = newWidth - 350;
            dlgGrid.Height = newHeight - 150;
        }

        private void AddSubject_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SubjectSource item = selectionTreeView.SelectedItem as SubjectSource;
            if (item == null) return;
            DownloadViewModel vm = DataContext as DownloadViewModel;
            vm.AddSelectedSubjectCommand.Execute(item);
        }
    }
}
