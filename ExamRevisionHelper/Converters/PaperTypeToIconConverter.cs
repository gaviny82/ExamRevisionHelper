using MaterialDesignThemes.Wpf;
using ExamRevisionHelper.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ExamRevisionHelper.Converters
{
    class PaperTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new PackIcon
            {
                Kind = ((ResourceType)value) switch
                {
                    ResourceType.QuestionPaper => PackIconKind.FileDocumentEdit,
                    ResourceType.Insert => PackIconKind.FileDocument,
                    ResourceType.ListeningAudio => PackIconKind.FileMusic,
                    ResourceType.SpeakingTestCards => PackIconKind.CardText,
                    ResourceType.Transcript => PackIconKind.FileReplace,
                    ResourceType.TeachersNotes => PackIconKind.FileUser,
                    ResourceType.ConfidentialInstructions => PackIconKind.FileLock,
                    ResourceType.MarkScheme => PackIconKind.FileTick,
                    _ => PackIconKind.FileDocument,
                }
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
