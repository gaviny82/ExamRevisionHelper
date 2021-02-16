using PastPaperHelper.Core.Tools;
using PastPaperHelper.Models;
using PastPaperHelper.Views;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PastPaperHelper.ViewModels
{
    public class SubjectDialogViewModel : BindableBase
    {
        public static ObservableCollection<Subject> IGSubjects { get; set; } = new ObservableCollection<Subject>();
        public static ObservableCollection<Subject> ALSubjects { get; set; } = new ObservableCollection<Subject>();
        public static void RefreshSubjectLists()
        {
            IGSubjects.Clear();
            ALSubjects.Clear();
            foreach (Subject item in PastPaperHelperCore.SubjectsLoaded)
            {
                if (item.Curriculum == Curriculums.IGCSE) IGSubjects.Add(item);
                else ALSubjects.Add(item);
            }
        }

        public SubjectDialogViewModel()
        {

        }

        #region SubscribeSubjectCommand
        private DelegateCommand<object> _subscribeSubjectCommand;
        public DelegateCommand<object> SubscribeSubjectCommand =>
            _subscribeSubjectCommand ?? (_subscribeSubjectCommand = new DelegateCommand<object>(ExecuteSubscribeSubjectCommand));

        private ConcurrentQueue<Subject> subjectPending = new ConcurrentQueue<Subject>();

        private bool isLoading = false;

        void ExecuteSubscribeSubjectCommand(object parameter)
        {
            if (!(parameter is Subject subj)) return;

            subjectPending.Enqueue(subj);
            if (!isLoading)
            {
                _ = trySubscribeLoop();
            }
        }

        private async Task trySubscribeLoop()
        {
            isLoading = true;
            Application.Current.MainWindow.Resources["IsLoading"] = Visibility.Visible;

            while (subjectPending.TryDequeue(out Subject subj))
            {
                await PastPaperHelperUpdateService.SubscribeAsync(subj);
            }
            await PastPaperHelperCore.SaveDataAsync();
            isLoading = false;
            Application.Current.MainWindow.Resources["IsLoading"] = Visibility.Hidden;
        }
        #endregion
    }
}
