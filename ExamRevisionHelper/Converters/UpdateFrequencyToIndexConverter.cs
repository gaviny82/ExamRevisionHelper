using System;
using System.Globalization;
using System.Windows.Data;
using ExamRevisionHelper.Core;

namespace ExamRevisionHelper.Converters
{
    public class UpdateFrequencyToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (UpdateFrequency)value;
        }
    }
}
