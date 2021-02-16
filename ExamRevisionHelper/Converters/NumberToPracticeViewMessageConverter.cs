using System;
using System.Globalization;
using System.Windows.Data;

namespace ExamRevisionHelper.Converters
{
    public class NumberToPracticeViewMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int num = (int)value;
            return $"You have done {num} paper{ (num > 1 ? "s" : "") }.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
