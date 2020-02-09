using PastPaperHelper.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            if (tab.SelectedIndex == 3)
            {
                var selection = (source.SelectedItem as ComboBoxItem).Content as string;
                var split = selection.Split('(');
                (DataContext as FirstRunWindowViewModel).LoadSubjectsCommand.Execute(split.First().Trim());
            }
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            tab.SelectedIndex--;
        }
    }
}
