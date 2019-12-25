namespace PastPaperHelper.Models
{
    public enum Curriculums { IGCSE, ALevel }

    public struct Subject
    {
        public Curriculums Curriculum { get; set; }
        public string Name { get; set; }
        public string SyllabusCode { get; set; }
    }

}
