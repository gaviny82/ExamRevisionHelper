using System.Collections.ObjectModel;

namespace PastPaperHelper.Models
{
    public class PaperRepository : ObservableCollection<ExamYear>
    {
        public Subject Subject { get; set; }

        public PaperRepository(Subject subject)
        {
            Subject = subject;
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

        public void Sort()
        {
            for (int i = 0; i < Count - 1; i++)
            {
                bool flag = true;
                for (int j = 0; j < Count - i - 1; j++)
                {
                    int.TryParse(this[j].Year, out int year1);
                    int.TryParse(this[j + 1].Year, out int year2);
                    if (year1 < year2)
                    {
                        ExamYear tmp = this[j];
                        this[j] = this[j + 1];
                        this[j + 1] = tmp;
                        flag = false;
                    }
                }
                if (flag) return;
            }
        }
    }
}
