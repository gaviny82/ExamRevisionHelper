using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ExamRevisionHelper.Core.Models;

namespace ExamRevisionHelper.Converters
{
    public class IsSyllabusUpdatedToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null) return Visibility.Hidden;
            return (values[0] as Syllabus).Year == (values[1] as string) ? Visibility.Visible : Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
