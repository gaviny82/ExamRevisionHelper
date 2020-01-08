using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace PastPaperHelper.PrismTest.Views
{
    /// <summary>
    /// Interaction logic for SettingsView
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void OpenReleases_Click(object sender, RoutedEventArgs e) => Process.Start("https://github.com/GavinYou082/PastPaperHelper/releases");

        private void OpenGithub_Click(object sender, RoutedEventArgs e) => Process.Start("https://github.com/GavinYou082/PastPaperHelper");

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => Process.Start("https://www.e-iceblue.com/Introduce/free-pdf-component.html");

    }
}
