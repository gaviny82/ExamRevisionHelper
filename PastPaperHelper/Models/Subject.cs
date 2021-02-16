using System;

namespace PastPaperHelper.Models
{
    public enum Curriculums { IGCSE, ALevel }

    public class Subject
    {
        public Curriculums Curriculum { get; set; }
        public string Name { get; set; }
        public string SyllabusCode { get; set; }

        public override bool Equals(object obj)
        {
            if(obj is Subject subj)
            {
                return subj.SyllabusCode == SyllabusCode;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int.TryParse(SyllabusCode, out int i);
            return i;
        }

        public static bool operator ==(Subject left, Subject right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.SyllabusCode.Equals(right.SyllabusCode);
        }

        public static bool operator !=(Subject left, Subject right)
        {
            return !(left == right);
        }
    }

}
