using PastPaperHelper.Sources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PastPaperHelper
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

        private void OpenReleases_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/GavinYou082/PastPaperHelper/releases");
        }

        private void OpenGithub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/GavinYou082/PastPaperHelper");
        }
    }

    public class PaperSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == PaperSources.GCE_Guide) return 0;
            else if (value == PaperSources.PapaCambridge) return 1;
            else return 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)value)
            {
                default:return null;
                case 0: return PaperSources.GCE_Guide;
                case 1: return PaperSources.PapaCambridge;
                case 2: return PaperSources.CIE_Notes;
            }
        }
    }
}
