using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace PastPaperHelper.Converters
{
    public class UrlToDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string url = value as string;
            return url.Split('/').Last();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
