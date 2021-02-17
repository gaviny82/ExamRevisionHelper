using System;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace ExamRevisionHelper.Converters
{
    public class UrlToFileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
         => (value as string)?.Split('/').Last();

        public object ConvertBack(object value, Type targetType, object parameter, string language) => null;
    }
}
