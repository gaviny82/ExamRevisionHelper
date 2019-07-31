using MaterialDesignThemes.Wpf;
using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using PastPaperHelper.ViewModels;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

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

        private void OpenReleases_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/GavinYou082/PastPaperHelper/releases");
        }

        private void OpenGithub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/GavinYou082/PastPaperHelper");
        }

        private void RootDlg_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newWidth = e.NewSize.Width, newHeight = e.NewSize.Height;
            dlgGrid.Width = newWidth - 350;
            dlgGrid.Height = newHeight - 150;
        }

        private void AddSubject_Click(object sender, RoutedEventArgs e)
        {
            SubjectSource item = selectionTreeView.SelectedItem as SubjectSource;
            if (item == null) return;
            SettingsViewModel vm = DataContext as SettingsViewModel;
            vm.AddSelectedSubjectCommand.Execute(item);
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
