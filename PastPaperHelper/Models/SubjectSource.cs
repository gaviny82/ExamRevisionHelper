namespace PastPaperHelper.Models
{
    public class SubjectSource
    {
        public Curriculums Curriculum { get; set; }

        public string Name { get; set; }

        public string SyllabusCode { get; set; }

        public string Url { get; set; }
    }

    public enum Curriculums { IGCSE, ALevel }
}
