namespace PastPaperHelper.Models
{
    public class PastPaperResource
    {
        public string Url { get; set; }
    }

    public class Paper : PastPaperResource
    {
        public Exam Exam { get; set; }
        public char Component { get; set; }
        public char Variant { get; set; }
        public FileTypes Type { get; set; }

        public Paper() { }

        public Paper(string fileName, Exam exam, string uri)
        {
            string[] split = fileName.Substring(0, fileName.Length - 4).Split('_');

            if (split.Length < 3) return;

            Exam = exam;
            Url = uri;

            switch (split[2])
            {
                default: Type = FileTypes.Unknown; break;
                case "ir": Type = FileTypes.ConfidentialInstructions; break;
                case "ci": Type = FileTypes.ConfidentialInstructions; break;
                case "su": Type = FileTypes.ListeningAudio; break;
                case "sf": Type = FileTypes.ListeningAudio; break;
                case "ms": Type = FileTypes.MarkScheme; break;
                case "qp": Type = FileTypes.QuestionPaper; break;
                case "rp": Type = FileTypes.SpeakingTestCards; break;
                case "tn": Type = FileTypes.TeachersNotes; break;
                case "qr": Type = FileTypes.Transcript; break;
                case "in": Type = FileTypes.Insert; break;
                case "in2": Type = FileTypes.Insert; break;
                case "i2": Type = FileTypes.Insert; break;
            }
            //TODO: consider the last 2-digit split part as the component code
            if (split.Length > 3)
            {
                Component = split[3][0];
                if (split[3].Length > 1) Variant = split[3][1];
            }

        }
    }

    public class Syllabus : PastPaperResource
    {
        public string Year { get; set; }
    }

    public class GradeThreshold : PastPaperResource
    {
        public Exam Exam { get; set; }
    }

    public class ExaminersReport : PastPaperResource
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
