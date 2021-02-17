using ExamRevisionHelper.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExamRevisionHelper.Views
{
    /// <summary>
    /// MarkPaperView.xaml 的交互逻辑
    /// </summary>
    public partial class MarkPaperView : UserControl
    {
        public MarkPaperView()
        {
            InitializeComponent();
            list.ItemsSource = (DataContext as MarkPaperViewModel).Questions;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if((e.Key<Key.NumPad0||e.Key>Key.NumPad9) && (e.Key < Key.D0 || e.Key > Key.D9))
            {
                e.Handled = true;
            }
        }

        private void Discard_Click(object sender, RoutedEventArgs e)
        {
            (App.Current.MainWindow.DataContext as MainWindowViewModel).NavigateCommand.Execute("Practice");
        }
    }
}
