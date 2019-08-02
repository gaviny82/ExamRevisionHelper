using HtmlAgilityPack;
using PastPaperHelper.Models;
using System;

namespace PastPaperHelper.Sources
{
    class PaperSourceCIENotes : PaperSource
    {
        public PaperSourceCIENotes()
        {
            throw new NotImplementedException();
            Name = "CIE Notes";
            Url = "https://papers.gceguide.com/";
        }

        public override PaperRepository GetPapers(SubjectSource subject)
        {
            throw new NotImplementedException();
        }

        public override SubjectSource[] GetSubjects(Curriculums? curriculum = null)
        {
            throw new NotImplementedException();
        }
    }
}
