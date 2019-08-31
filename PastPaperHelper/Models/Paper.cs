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
        public ResourceType Type { get; set; }

        public Paper() { }

        public Paper(string fileName, Exam exam, string uri)
        {
            string[] split = fileName.Substring(0, fileName.Length - 4).Split('_');

            if (split.Length < 3) return;

            Exam = exam;
            Url = uri;

            switch (split[2])
            {
                default: Type = ResourceType.Unknown; break;
                case "ir": Type = ResourceType.ConfidentialInstructions; break;
                case "ci": Type = ResourceType.ConfidentialInstructions; break;
                case "su": Type = ResourceType.ListeningAudio; break;
                case "sf": Type = ResourceType.ListeningAudio; break;
                case "ms": Type = ResourceType.MarkScheme; break;
                case "qp": Type = ResourceType.QuestionPaper; break;
                case "rp": Type = ResourceType.SpeakingTestCards; break;
                case "tn": Type = ResourceType.TeachersNotes; break;
                case "qr": Type = ResourceType.Transcript; break;
                case "in": Type = ResourceType.Insert; break;
                case "in2": Type = ResourceType.Insert; break;
                case "i2": Type = ResourceType.Insert; break;
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
    public enum ResourceType
    {
        ExaminersReport,
        GradeThreshold,
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
