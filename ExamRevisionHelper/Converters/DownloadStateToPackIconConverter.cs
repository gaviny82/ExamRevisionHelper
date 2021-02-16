using MaterialDesignThemes.Wpf;
using ExamRevisionHelper.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ExamRevisionHelper.Converters
{
    public class DownloadStateToPackIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (DownloadTaskState)value switch
            {
                DownloadTaskState.Downloading => PackIconKind.Download,
                DownloadTaskState.Completed => PackIconKind.Done,
                DownloadTaskState.Error => PackIconKind.Error,
                _ => null,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class DownloadStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (DownloadTaskState)value == DownloadTaskState.Pending ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
