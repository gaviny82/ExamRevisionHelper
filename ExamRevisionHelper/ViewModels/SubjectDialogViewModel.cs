using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using ExamRevisionHelper.Core;
using ExamRevisionHelper.Core.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace ExamRevisionHelper.ViewModels
{
    public class SubjectDialogViewModel : BindableBase
    {
        public static ObservableCollection<Subject> IGSubjects { get; set; } = new ObservableCollection<Subject>();
        public static ObservableCollection<Subject> ALSubjects { get; set; } = new ObservableCollection<Subject>();
        public static void RefreshSubjectLists()
        {
            IGSubjects.Clear();
            ALSubjects.Clear();
            foreach (Subject item in App.CurrentInstance.SubjectsAvailable)
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
                await App.CurrentInstance.Updater.SubscribeAsync(subj);
            }
            await Task.Run(()=> 
            {
                XmlDocument doc = App.CurrentInstance.UserData;
                doc.Save($"{App.ConfigFolderPath}\\{App.CurrentSource.Name}.xml");
            });
            isLoading = false;
            Application.Current.MainWindow.Resources["IsLoading"] = Visibility.Hidden;
        }
        #endregion
    }
}
