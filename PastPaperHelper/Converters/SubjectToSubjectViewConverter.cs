using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using PastPaperHelper.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PastPaperHelper.Converters
{
    class SubjectToExamYearViewModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            Subject subject = (Subject)value;
            ExamYearsCollection collection = new ExamYearsCollection();
            PaperRepository repo = SubscriptionManager.Subscription[subject];

            foreach(Exam item in repo.Exams)
            {
                ExamYearViewModel year = collection.GetExamYear(item.Year);
                if (year == null)
                {
                    year = new ExamYearViewModel { Year = item.Year };
                    collection.Add(year);
                }
                switch (item.Series)
                {
                    case ExamSeries.Spring:
                        year.Spring = item;
                        break;
                    case ExamSeries.Summer:
                        year.Summer = item;
                        break;
                    case ExamSeries.Winter:
                        year.Winter = item;
                        break;
                    default:
                        year.Specimen = item;
                        break;
                }
            }

            collection.Sort();

            for (int i = 0; i < repo.Syllabus.Length; i++)
            {
                Syllabus syllabus = repo.Syllabus[i];
                ExamYearViewModel year = collection.GetExamYear(syllabus.Year);
                if (year != null)
                {
                    string yearEnd = "";
                    if (i < repo.Syllabus.Length - 1)
                    {
                        yearEnd = repo.Syllabus[i + 1].Year;
                    }

                    int index = collection.IndexOf(year);
                    while (index != -1 && collection[index].Year != yearEnd)
                    {
                        collection[index].Syllabus = syllabus;
                        index--;
                    }
                }
            }

            return collection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
