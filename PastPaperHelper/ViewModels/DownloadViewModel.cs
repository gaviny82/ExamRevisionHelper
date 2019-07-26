using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace PastPaperHelper
{
    public class DownloadViewModel : NotificationObject
    {
        public static ObservableCollection<SubjectSource> Subjects { get; } = new ObservableCollection<SubjectSource>();
        public static ObservableCollection<SubjectSource> IGSubjects { get; } = new ObservableCollection<SubjectSource>();
        public static ObservableCollection<SubjectSource> ALSubjects { get; } = new ObservableCollection<SubjectSource>();
        public DownloadViewModel()
        {
            RemoveSelectedSubjectsCommand = new DelegateCommand(RemoveSelectedSubjects);
            RemoveSubjectCommand = new DelegateCommand(RemoveSubject);
            AddSelectedSubjectCommand = new DelegateCommand(AddSelectedSubject);
        }

        private SubjectSource _selectedSubject;
        public SubjectSource SelectedSubject
        {
            get { return _selectedSubject; }
            set { _selectedSubject = value; RaisePropertyChangedEvent("SelectedSubject"); }
        }

        public DelegateCommand RemoveSubjectCommand { get; set; }
        private void RemoveSubject(object param)
        {
            string code = param as string;
            for (int i = 0; i < Subjects.Count; i++)
            {
                if (Subjects[i].SyllabusCode == code)
                {
                    Subjects.RemoveAt(i);
                    Properties.Settings.Default.SubjectsSubcripted.Remove(code);
                    Properties.Settings.Default.Save();
                    return;
                }
            }
        }

        public DelegateCommand RemoveSelectedSubjectsCommand { get; set; }
        private void RemoveSelectedSubjects(object param)
        {
            IList list = (IList)param;
            while (list.Count > 0)
            {
                SubjectSource subject = list[0] as SubjectSource;
                Subjects.Remove(subject);
                Properties.Settings.Default.SubjectsSubcripted.Remove(subject.SyllabusCode);
            }
            Properties.Settings.Default.Save();
        }


        public DelegateCommand AddSelectedSubjectCommand { get; set; }
        private void AddSelectedSubject(object param)
        {
            Dictionary<SubjectSource, PaperItem[]> item = SourceManager.Subscription;
            //TODO: Hot reload papers at download view
            SubjectSource subject = param as SubjectSource;
            if (!Subjects.Contains(subject))
            {
                Subjects.Add(subject);
                Properties.Settings.Default.SubjectsSubcripted.Add(subject.SyllabusCode);
                Properties.Settings.Default.Save();
            }
        }

        public static void RefreshSubjectList()
        {
            if (SourceManager.AllSubjects != null)
            {
                IGSubjects.Clear();
                ALSubjects.Clear();
                foreach (SubjectSource item in SourceManager.AllSubjects)
                {
                    if (item.Curriculum == Curriculums.ALevel)
                        ALSubjects.Add(item);
                    else
                        IGSubjects.Add(item);
                }
            }

            Subjects.Clear();
            foreach (KeyValuePair<SubjectSource, PaperItem[]> item in SourceManager.Subscription)
            {
                Subjects.Add(item.Key);
            }

        }
    }
}
