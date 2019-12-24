namespace PastPaperHelper.Models
{
    public class ExamYear
    {
        public string Year { get; set; }
        public Syllabus Syllabus { get; set; }
        public Exam Spring { get; set; }
        public Exam Summer { get; set; }
        public Exam Winter { get; set; }
        public Exam Specimen { get; set; }
    }
}
