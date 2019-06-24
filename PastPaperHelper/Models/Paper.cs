using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.Models
{
    public class PaperItem
    {
        public Subject Subject { get; set; }
        public string Year { get; set; }
        public ExamSeries ExamSeries { get; set; }
        public int ComponentCode { get; set; }
        public int VariantCode { get; set; }
        public FileTypes Type { get; set; }
    }

    public enum ExamSeries { Spring, Summer, Winter }
    public enum FileTypes { ExaminersReport, Syllabus, ConfidentialInstructions, TeachersNotes, ListeningAudio, SpeakingTestCards , GradeThreshold, QuestionPaper, MarkScheme, Insert, Transcript,  }
}
