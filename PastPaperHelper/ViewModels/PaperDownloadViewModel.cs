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
    public class PaperDownloadViewModel : ObservableObject
    {
        public ObservableCollection<Subject> Subjects { get; } = new ObservableCollection<Subject>();
        public PaperDownloadViewModel()
        {

            Subjects.Add(new Subject { Curriculum = Curriculums.IGCSE, Name = "Physics", SyllabusCode = "0625" });
            Subjects.Add(new Subject { Curriculum = Curriculums.ALevel, Name = "Mathematics", SyllabusCode = "9709" });
        }

        public ICommand RemoveSubjectCommand
        {
            get => new DelegateCommand(RemoveSubject);
        }
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

        public ICommand RemoveSelectedSubjectsCommand
        {
            get => new DelegateCommand(RemoveSelectedSubjects);
        }
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
