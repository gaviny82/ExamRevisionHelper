using System.Windows;

namespace PastPaperHelper.Views
{
    /// <summary>
    /// Interaction logic for FirstRunWindow.xaml
    /// </summary>
    public partial class FirstRunWindow : Window
    {
        public FirstRunWindow()
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
