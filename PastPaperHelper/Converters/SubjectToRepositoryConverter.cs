using PastPaperHelper.Core.Tools;
using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using PastPaperHelper.Views;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PastPaperHelper.Converters
{
    class SubjectToRepositoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Subject subject && PastPaperHelperCore.Source.Subscription.ContainsKey(subject))
            {
                return PastPaperHelperCore.Source.Subscription[subject];
            }
            else
            {
                var subsColl = Properties.Settings.Default.SubjectsSubcription;
                string[] subscribedSubjects = new string[subsColl.Count];
                subsColl.CopyTo(subscribedSubjects, 0);
                MainWindow.MainSnackbar.MessageQueue.Enqueue(
                    content: $"An error has occurred. Try reloading from {PastPaperHelperCore.Source.Name}",
                    actionContent: "RELOAD",
                    actionHandler: (param) => { PastPaperHelperUpdateService.UpdateAll(subscribedSubjects); }, null,
                    promote: true,
                    neverConsiderToBeDuplicate: true,
                    durationOverride: TimeSpan.FromDays(1));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
