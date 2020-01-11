using System;

namespace PastPaperHelper.Models
{
    public enum Curriculums { IGCSE, ALevel }

    public struct Subject
    {
        public Curriculums Curriculum { get; set; }
        public string Name { get; set; }
        public string SyllabusCode { get; set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int.TryParse(SyllabusCode, out int i);
            return i;
        }

        public static bool operator ==(Subject left, Subject right)
        {
            return left.SyllabusCode.Equals(right.SyllabusCode);
        }

        public static bool operator !=(Subject left, Subject right)
        {
            return !(left == right);
        }
    }

}
