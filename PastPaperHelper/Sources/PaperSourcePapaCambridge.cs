using HtmlAgilityPack;
using PastPaperHelper.Models;
using System;

namespace PastPaperHelper.Sources
{
    class PaperSourcePapaCambridge : PaperSource
    {
        public PaperSourcePapaCambridge()
        {
            throw new NotImplementedException();
            Name = "PapaCambridge";
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
