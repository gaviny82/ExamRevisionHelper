using PastPaperHelper.Core.Tools;
using PastPaperHelper.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

        void ExecuteSubscribeSubjectCommand(object parameter)
        {
            if (!(parameter is Subject subj)) return;
            
        }

        #endregion
    }
}
