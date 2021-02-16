using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ExamRevisionHelper.Views
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

    }
}
