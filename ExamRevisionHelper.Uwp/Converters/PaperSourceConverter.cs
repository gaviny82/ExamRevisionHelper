using System;
using Windows.UI.Xaml.Data;

namespace ExamRevisionHelper.Converters
{
    public class PaperSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
             => value switch
             {
                 "gce_guide" => 0,
                 "papacambridge" => 1,
                 "cie_notes" => 2,
                 _ => null,
             };

        public object ConvertBack(object value, Type targetType, object parameter, string language) 
            => value switch
            {
                0 => "gce_guide",
                1 => "papacambridge",
                2 => "cie_notes",
                _ => null,
            };
    }
}
