using PastPaperHelper.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.Models
{
    class ExamYearsCollection : ObservableCollection<ExamYear>
    {
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
