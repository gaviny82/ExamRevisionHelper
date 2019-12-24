using PastPaperHelper.ViewModels;
using System.Windows;

namespace PastPaperHelper.Views
{
    /// <summary>
    /// OobeView.xaml 的交互逻辑
    /// </summary>
    public partial class OobeWindow : Window
    {
        public OobeWindow()
        {
            InitializeComponent();
            DataContext = new OobeViewModel();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            tab.SelectedIndex++;
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            tab.SelectedIndex--;
        }
    }
}
