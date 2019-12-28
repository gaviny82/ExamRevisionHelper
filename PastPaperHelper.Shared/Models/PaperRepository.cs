using PastPaperHelper.Core.Tools;
using System.Collections.Generic;

namespace PastPaperHelper.Models
{
    public class PaperRepository : List<ExamYear>
    {
        public Subject Subject { get; set; }

        public PaperRepository(Subject subject)
        {
            Subject = subject;
        }
        public PaperRepository(string syllabusCode)
        {
            foreach(Subject subj in PastPaperHelperCore.SubjectsLoaded)
            {
                if (subj.SyllabusCode == syllabusCode)
                {
                    Subject = subj;
                    return;
                }
            }
        }

        public ExamYear this[string year] { get => GetExamYear(year); }
        public ExamYear GetExamYear(string Year)
        {
            foreach (ExamYear item in this)
            {
                if (item.Year == Year) return item;
            }
            return null;
        }
    }
}
