using HtmlAgilityPack;
using PastPaperHelper.Models;
using System;
using System.Collections.Generic;

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

        public override PaperRepository GetPapers(Subject subject, string url)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<Subject, string> GetSubjectUrlMap(Curriculums curriculum)
        {
            throw new NotImplementedException();
        }
    }
}
