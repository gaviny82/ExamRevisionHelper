namespace ExamRevisionHelper.Core.Models
{
    public class Question
    {
        public string SyllabusCode { get; set; }
        public string Year { get; set; }
        public ExamSeries Series { get; set; }
        public string PaperCode { get; set; }
        public int QuestionNumber { get; set; }

        public string FilePath { get; set; }

        public Question() { }

        public Question(string fileName, int questionNumber)
        {
            string[] split = fileName.Split('_');
            SyllabusCode = split[0];
            switch (split[1][0])
            {
                case 'm': Series = ExamSeries.Spring; break;
                case 's': Series = ExamSeries.Summer; break;
                case 'w': Series = ExamSeries.Winter; break;
            }
            Year = "20" + split[1].Substring(1);
            PaperCode = fileName.Substring(9).Replace(".pdf", "").Replace("_", "").Replace("qp", "");
            QuestionNumber = questionNumber;
        }
    }
}
