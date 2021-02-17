using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using ExamRevisionHelper.Core.Models;

namespace ExamRevisionHelper.Core.Sources
{
    public class PaperSourceCIENotes : PaperSource
    {
        public PaperSourceCIENotes()
        {
            throw new NotImplementedException();
            Name = "CIE Notes";
            UrlBase = "https://papers.gceguide.com/";
        }
        public PaperSourceCIENotes(XmlDocument data) : base(data)
        {
            throw new NotImplementedException();
            Name = "CIE Notes";
            UrlBase = "https://papers.gceguide.com/";
        }

        public override Task<PaperRepository> GetPapers(Subject subject)
        {
            throw new NotImplementedException();
        }

        public override async Task<Dictionary<Subject, string>> GetSubjectUrlMapAsync(Curriculums curriculum)
        {
            throw new NotImplementedException();
        }
    }
}
