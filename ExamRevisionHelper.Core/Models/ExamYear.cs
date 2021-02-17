using System;

namespace ExamRevisionHelper.Core.Models
{
    public class ExamYear : IComparable<ExamYear>
    {
        public string Year { get; set; }
        public Syllabus Syllabus { get; set; }
        public Exam Spring { get; set; }
        public Exam Summer { get; set; }
        public Exam Winter { get; set; }
        public Exam Specimen { get; set; }

        public int CompareTo(ExamYear other)
        {
            int.TryParse(Year, out int year);
            int.TryParse(other.Year, out int year2);
            return year.CompareTo(year2);
        }
    }
}
