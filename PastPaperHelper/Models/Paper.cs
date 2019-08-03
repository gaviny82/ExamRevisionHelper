namespace PastPaperHelper.Models
{
    public class PaperItem
    {
        public Exam Exam { get; set; }
        public char ComponentCode { get; set; }
        public char VariantCode { get; set; }
        public FileTypes Type { get; set; }
        public string Url { get; set; }

        public PaperItem() { }
        public PaperItem(Exam exam, char componentCode, char variantCode, FileTypes type)
        {
            Exam = exam;
            ComponentCode = componentCode;
            VariantCode = variantCode;
            Type = type;
        }
    }

    public enum ExamSeries { Spring, Summer, Winter, Specimen }
    public enum FileTypes
    {
        ExaminersReport,
        Syllabus,
        ConfidentialInstructions,
        TeachersNotes,
        ListeningAudio,
        SpeakingTestCards ,
        GradeThreshold,
        QuestionPaper,
        MarkScheme,
        Insert,
        Transcript,
        Unknown
    }
}
