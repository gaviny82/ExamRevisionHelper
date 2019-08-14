namespace PastPaperHelper.Models
{
    public class Exam
    {
        public Subject Subject { get; set; }
        public string Year { get; set; }
        public ExamSeries Series { get; set; }
        public GradeThreshold GradeThreshold { get; set; }
        public ExaminersReport ExaminersReport { get; set; }
        public Paper[] Papers { get; set; }
    }

    public enum Curriculums { IGCSE, ALevel }
}
