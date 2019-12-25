using Common.Wpf.Data;
using System.Linq;

namespace PastPaperHelper.Models
{
    public enum ResourceStates { Online, Downloading, Offline }
    public class PastPaperResource : TreeGridElement
    {
        public string Url { get; set; }
        public ResourceStates State { get; set; } = ResourceStates.Online;
        public string Path { get; set; }
    }

    public enum ResourceType
    {
        QuestionPaper,
        Insert,
        MarkScheme,
        ListeningAudio,
        SpeakingTestCards,
        Transcript,
        TeachersNotes,
        ConfidentialInstructions,
        ExaminersReport,
        GradeThreshold,
        Unknown
    }

    public class Paper : PastPaperResource
    {
        public Exam Exam { get; set; }
        public char Component { get; set; }
        public char Variant { get; set; }
        public ResourceType Type { get; set; }

        public Paper() { }

        static readonly char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '0' };
        public Paper(string fileName, Exam exam, string uri)
        {
            string[] split = fileName[0..^4].Split('_');

            if (split.Length < 3) return;

            Exam = exam;
            Url = uri;
            Type = (split[2]) switch
            {
                "ir" => ResourceType.ConfidentialInstructions,
                "ci" => ResourceType.ConfidentialInstructions,
                "su" => ResourceType.ListeningAudio,
                "sf" => ResourceType.ListeningAudio,
                "ms" => ResourceType.MarkScheme,
                "qp" => ResourceType.QuestionPaper,
                "rp" => ResourceType.SpeakingTestCards,
                "tn" => ResourceType.TeachersNotes,
                "qr" => ResourceType.Transcript,
                "in" => ResourceType.Insert,
                "in2" => ResourceType.Insert,
                "i2" => ResourceType.Insert,
                _ => ResourceType.Unknown,
            };

            //finding component and variant code of this paper from its file name
            int index = fileName.Length;
            while (--index > 1)
            {
                //check each character from the end to find the last number
                char ch = fileName[index];
                if (numbers.Contains(ch))
                {
                    char prevCh = fileName[index - 1];
                    if (numbers.Contains(prevCh))
                    {
                        //file name with component+variant
                        Component = prevCh;
                        Variant = ch;
                        break;
                    }
                    else
                    {
                        //file name with component only
                        Component = ch;
                        Variant = '0';
                        break;
                    }
                }
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

}
