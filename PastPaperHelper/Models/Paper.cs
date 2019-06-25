namespace PastPaperHelper.Models
{
    public class PaperItem
    {
        public SubjectSource Subject { get; set; }
        public string Year { get; set; }
        public ExamSeries ExamSeries { get; set; }
        public char ComponentCode { get; set; }
        public char VariantCode { get; set; }
        public FileTypes Type { get; set; }
        public string Url { get; set; }
    }

    public enum ExamSeries { Spring, Summer, Winter }
    public enum FileTypes { ExaminersReport, Syllabus, ConfidentialInstructions, TeachersNotes, ListeningAudio, SpeakingTestCards , GradeThreshold, QuestionPaper, MarkScheme, Insert, Transcript, Unknown }
}
