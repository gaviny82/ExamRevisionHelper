using PastPaperHelper.Sources;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PastPaperHelper.Converters
{
    public class PaperSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                "gce_guide" => 0,
                "papacambridge" => 1,
                "cie_notes" => 2,
                _ => null,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                0 => "gce_guide",
                1 => "papacambridge",
                2 => "cie_notes",
                _ => null,
            };
        }
    }
}
