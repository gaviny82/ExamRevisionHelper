using System;
using System.Globalization;
using System.Windows.Data;

namespace ExamRevisionHelper.Converters
{
    public class SecondToHourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int hour= ((int)value / 60 / 60);
            return string.Format("{0:D2}", hour);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class SecondToMinuteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int min = (int)value / 60 % 60;
            return string.Format("{0:D2}", min);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class SecondToDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int sec = (int)value % 60 % 60;
            return string.Format("{0:D2}", sec);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
