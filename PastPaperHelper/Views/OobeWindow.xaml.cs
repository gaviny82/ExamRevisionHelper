using System.Windows;

namespace PastPaperHelper.Views
{
    /// <summary>
    /// Interaction logic for OobeWindow.xaml
    /// </summary>
    public partial class OobeWindow : Window
    {
        public OobeWindow()
        {
            InitializeComponent();
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
