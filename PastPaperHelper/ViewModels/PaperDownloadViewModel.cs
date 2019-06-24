using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PastPaperHelper
{
    public class PaperDownloadViewModel : NotificationObject
    {
        public ObservableCollection<Subject> Subjects { get; } = new ObservableCollection<Subject>();
        public PaperDownloadViewModel()
        {
            RemoveSelectedSubjectsCommand = new DelegateCommand(RemoveSelectedSubjects);
            RemoveSubjectCommand = new DelegateCommand(RemoveSubject);

            Subjects.Add(new Subject { Curriculum = Curriculums.IGCSE, Name = "Physics", SyllabusCode = "0625" });
            Subjects.Add(new Subject { Curriculum = Curriculums.ALevel, Name = "Mathematics", SyllabusCode = "9709" });
        }

        private Subject _selectedSubject;
        public Subject SelectedSubject
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
                Subjects.Remove((Subject)list[0]);
            }
        }
    }
}
