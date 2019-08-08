using PastPaperHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.Models
{
    class ExamYear
    {
        public string Year { get; set; }
        public Syllabus Syllabus { get; set; }
        public Exam Spring { get; set; }
        public Exam Summer { get; set; }
        public Exam Winter { get; set; }
        public Exam Specimen { get; set; }
    }
}
