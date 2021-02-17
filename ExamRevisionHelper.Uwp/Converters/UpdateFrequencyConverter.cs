using System;
using Windows.UI.Xaml.Data;

namespace ExamRevisionHelper.Converters
{
    public class UpdateFrequencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
             => (int)(UpdateFrequency)value;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => (UpdateFrequency)(int)value;
    }
}
