using ExamRevisionHelper.Core.Tools;
using ExamRevisionHelper.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ExamRevisionHelper.Views
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
                Properties.Settings.Default.UpdatePolicy = updateFreqSelector.SelectedIndex;

                var selection = (source.SelectedItem as ComboBoxItem).Content as string;
                var split = selection.Split('(');
                (DataContext as FirstRunWindowViewModel).LoadSubjectsCommand.Execute(split.First().Trim());
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
