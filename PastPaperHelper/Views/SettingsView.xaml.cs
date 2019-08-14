using MaterialDesignThemes.Wpf;
using PastPaperHelper.Models;
using PastPaperHelper.ViewModels;
using System.Diagnostics;
using System.Windows;

namespace PastPaperHelper.Views
{
    /// <summary>
    /// Config.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsView : DialogHost
    {
        //TODO: Drop folder to select path
        //TODO: Better scroll
        public SettingsView()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }

        private void OpenReleases_Click(object sender, RoutedEventArgs e) => Process.Start("https://github.com/GavinYou082/PastPaperHelper/releases");

        private void OpenGithub_Click(object sender, RoutedEventArgs e) => Process.Start("https://github.com/GavinYou082/PastPaperHelper");

        private void RootDlg_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newWidth = e.NewSize.Width, newHeight = e.NewSize.Height;
            dlgGrid.Width = newWidth - 350;
            dlgGrid.Height = newHeight - 150;
        }

        private void AddSubject_Click(object sender, RoutedEventArgs e)
        {
            if (!(selectionTreeView.SelectedItem is Subject)) return;
            Subject item = (Subject)selectionTreeView.SelectedItem;
            SettingsViewModel vm = DataContext as SettingsViewModel;
            vm.AddSelectedSubjectCommand.Execute(item);
        }

    }
}
