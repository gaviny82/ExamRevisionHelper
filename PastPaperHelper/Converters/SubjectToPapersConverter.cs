using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PastPaperHelper.Converters
{
    class SubjectToPapersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Subject)
            {
                Subject subject = (Subject)value;
                return SubscriptionManager.Subscription[subject];
            }
            else return null;            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
