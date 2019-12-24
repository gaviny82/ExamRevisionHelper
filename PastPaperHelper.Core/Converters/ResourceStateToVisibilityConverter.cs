using PastPaperHelper.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PastPaperHelper.Converters
{
    public class ResourceStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ResourceStates)value == ResourceStates.Online ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
