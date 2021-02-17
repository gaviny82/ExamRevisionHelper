using ExamRevisionHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ExamRevisionHelper.Converters
{
    public class ShortCurriculumNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            return ((Curriculums)value) switch
            {
                Curriculums.IGCSE => "IG",
                Curriculums.ALevel => "AL",
                _ => "",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return (value?.ToString()) switch
            {
                "IG" => Curriculums.IGCSE,
                "AL" => Curriculums.ALevel,
                _ => null,
            };
        }
    }
}
