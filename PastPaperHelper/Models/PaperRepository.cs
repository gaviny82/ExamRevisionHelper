using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.Models
{
    public class PaperRepository
    {
        public Subject Subject { get; set; }
        public PaperItem[] Syllabus { get; set; }
        public Exam[] Exams { get; set; }

        public PaperRepository(Subject subject)
        {
            Subject = subject;
        }

    }
}
