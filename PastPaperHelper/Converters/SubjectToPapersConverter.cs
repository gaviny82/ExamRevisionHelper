using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PastPaperHelper.Converters
{
    class SubjectToPapersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PaperSource PaperSource;
            switch (Properties.Settings.Default.PaperSource)
            {
                default: PaperSource = PaperSources.GCE_Guide; break;
                case "GCE Guide": PaperSource = PaperSources.GCE_Guide; break;
                case "PapaCambridge": PaperSource = PaperSources.PapaCambridge; break;
                case "CIE Notes": PaperSource = PaperSources.CIE_Notes; break;
            }
            SubjectSource subject = value as SubjectSource;
            if (subject != null)
            {
                PaperItem[] papers = PaperSource.GetPapers(subject);
                return papers;
            }
            else return null;            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
