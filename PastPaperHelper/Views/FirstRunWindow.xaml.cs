using PastPaperHelper.Core.Tools;
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
            if (tab.SelectedIndex == 2)
            {
                var selection = (source.SelectedItem as ComboBoxItem).Content as string;
                var split = selection.Split('(');
                var param = (split.First().Trim(), (UpdateFrequency)updateFreqSelector.SelectedIndex);
                (DataContext as FirstRunWindowViewModel).LoadSubjectsCommand.Execute(param);
            }
            else if (tab.SelectedIndex == 3)
            {
                (DataContext as FirstRunWindowViewModel).InitializeRepoCommand.Execute();
            }
            tab.SelectedIndex++;
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            tab.SelectedIndex--;
        }
    }
}
