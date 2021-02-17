using ExamRevisionHelper.Models;
using System;
using Windows.UI.Xaml.Data;

namespace ExamRevisionHelper.Converters
{
    class PaperTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return "File";
            switch ((ResourceType)value)
            {
                //case ResourceType.QuestionPaper:
                //    return new PackIcon { Kind = PackIconKind.FileDocumentEdit };
                //case ResourceType.Insert:
                //    return new PackIcon { Kind = PackIconKind.FileDocument };
                //case ResourceType.ListeningAudio:
                //    return new PackIcon { Kind = PackIconKind.FileMusic };
                //case ResourceType.SpeakingTestCards:
                //    return new PackIcon { Kind = PackIconKind.CardText };
                //case ResourceType.Transcript:
                //    return new PackIcon { Kind = PackIconKind.FileReplace };
                //case ResourceType.TeachersNotes:
                //    return new PackIcon { Kind = PackIconKind.FileUser };
                //case ResourceType.ConfidentialInstructions:
                //    return new PackIcon { Kind = PackIconKind.FileLock };
                //case ResourceType.MarkScheme:
                //    return new PackIcon { Kind = PackIconKind.FileTick };
                //default:
                //    return new PackIcon { Kind = PackIconKind.FileDocument };
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => null;
    }
}
