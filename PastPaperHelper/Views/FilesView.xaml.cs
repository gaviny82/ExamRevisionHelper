using PastPaperHelper.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView view = sender as ListView;
            if (view.SelectedItem != null)
            {
                (DataContext as FilesViewModel).OpenOnlineResourceCommand.Execute(view.SelectedItem);
            }
        }
    }
}
