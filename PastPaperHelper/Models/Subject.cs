using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PastPaperHelper
{
    public class Subject
    {
        public Curriculums Curriculum { get; set; }
        public string Name { get; set; }
        public string SyllabusCode { get; set; }
    }

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

    public class MessageVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public enum Curriculums { IGCSE, ALevel }
}
