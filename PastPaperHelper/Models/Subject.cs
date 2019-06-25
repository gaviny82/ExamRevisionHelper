using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

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
