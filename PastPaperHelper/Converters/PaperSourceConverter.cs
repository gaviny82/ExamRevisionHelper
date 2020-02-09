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
            if (!(value is string name)) return null;

            //if (name.StartsWith(PaperSources.GCE_Guide)) return 0;
            //else if (value == PaperSources.PapaCambridge) return 1;
            //else return 2;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)value)
            {
                default: return null;
                //case 0: return PaperSources.GCE_Guide;
                //case 1: return PaperSources.PapaCambridge;
                //case 2: return PaperSources.CIE_Notes;
            }
        }
    }
}
