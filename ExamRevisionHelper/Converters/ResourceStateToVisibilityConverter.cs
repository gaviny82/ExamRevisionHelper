using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ExamRevisionHelper.Core.Models;

namespace ExamRevisionHelper.Converters
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
