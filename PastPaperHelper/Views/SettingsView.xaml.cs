using MaterialDesignThemes.Wpf;
using PastPaperHelper.Models;
using PastPaperHelper.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace PastPaperHelper.Views
{
    /// <summary>
    /// Config.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsView : Grid
    {
        //TODO: Drop folder to select path
        public SettingsView()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }

        private void OpenReleases_Click(object sender, RoutedEventArgs e) => Process.Start("https://github.com/GavinYou082/PastPaperHelper/releases");

        private void OpenGithub_Click(object sender, RoutedEventArgs e) => Process.Start("https://github.com/GavinYou082/PastPaperHelper");

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => Process.Start("https://www.e-iceblue.com/Introduce/free-pdf-component.html");
    }
}
