using ExamRevisionHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ExamRevisionHelper.Converters
{
    class SubjectToRepositoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is Subject subject && App.PaperSource.Subscription.ContainsKey(subject))
            {
                return App.PaperSource.Subscription[subject];
            }
            //else
            //{
            //    var subsColl = Properties.Settings.Default.SubjectsSubcription;
            //    string[] subscribedSubjects = new string[subsColl.Count];
            //    subsColl.CopyTo(subscribedSubjects, 0);
            //    MainWindow.MainSnackbar.MessageQueue.Enqueue(
            //        content: $"An error has occurred. Try reloading from {PastPaperHelperCore.Source.DisplayName}",
            //        actionContent: "RELOAD",
            //        actionHandler: (param) => { PastPaperHelperUpdateService.UpdateAll(subscribedSubjects); }, null,
            //        promote: true,
            //        neverConsiderToBeDuplicate: true,
            //        durationOverride: TimeSpan.FromDays(1));
            //}
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return null;
        }
    }
}
