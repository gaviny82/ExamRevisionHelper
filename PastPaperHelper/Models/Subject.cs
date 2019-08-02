using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.Models
{
    public class Subject
    {
        public Curriculums Curriculum { get; set; }
        public string Name { get; set; }
        public string SyllabusCode { get; set; }

        public Subject() { }
        public Subject(Curriculums curriculum, string name, string syllabusCode)
        {
            Curriculum = curriculum;
            Name = name;
            SyllabusCode = syllabusCode;
        }
    }

}
