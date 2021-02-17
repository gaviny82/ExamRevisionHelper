using System;

namespace ExamRevisionHelper.Utils
{
    public class SubjectUnsupportedException : Exception
    {
        public string[] UnsupportedSubjects { get; set; }

        public SubjectUnsupportedException() { }
        public SubjectUnsupportedException(string msg) : base(msg) { }

        public SubjectUnsupportedException(string message, Exception innerException) : base(message, innerException) { }
    }

}
