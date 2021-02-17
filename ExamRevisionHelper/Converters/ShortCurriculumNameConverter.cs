using System;
using System.Globalization;
using System.Windows.Data;
using ExamRevisionHelper.Core.Models;

namespace ExamRevisionHelper.Converters
{

    public class ShortCurriculumNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Curriculums)value)
            {
                default: return "";
                case Curriculums.IGCSE: return "IG";
                case Curriculums.ALevel: return "AL";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString())
            {
                default: return null;
                case "IG": return Curriculums.IGCSE;
                case "AL": return Curriculums.ALevel;
            }
        }
    }
}
