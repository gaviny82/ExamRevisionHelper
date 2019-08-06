namespace PastPaperHelper.Models
{
    public class PastPaperResource
    {
        public string Url { get; set; }
    }

    public class Paper : PastPaperResource
    {
        public Exam Exam { get; set; }
        public char ComponentCode { get; set; }
        public char VariantCode { get; set; }
        public FileTypes Type { get; set; }
    }

    public class Syllabus : PastPaperResource
    {
        public string Year { get; set; }
    }

    public class GradeThreshold : PastPaperResource
    {
        public Exam Exam { get; set; }
    }


    public enum ExamSeries { Spring, Summer, Winter, Specimen }
    public enum FileTypes
    {
        ExaminersReport,
        ConfidentialInstructions,
        TeachersNotes,
        ListeningAudio,
        SpeakingTestCards ,
        QuestionPaper,
        MarkScheme,
        Insert,
        Transcript,
        Unknown
    }
}
