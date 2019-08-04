using HtmlAgilityPack;
using PastPaperHelper.Models;
using System;
using System.Collections.Generic;

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

        public override PaperRepository GetPapers(Subject subject, string url)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<Subject, string> GetSubjectUrlMap(Curriculums? curriculum = null)
        {
            throw new NotImplementedException();
        }
    }
}
