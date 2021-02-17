using System;
using Windows.UI.Xaml.Data;

namespace ExamRevisionHelper.Converters
{
    public class NotNullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
         => value != null;

        public object ConvertBack(object value, Type targetType, object parameter, string language) => null;
    }
}
