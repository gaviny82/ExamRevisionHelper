using ExamRevisionHelper.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace ExamRevisionHelper.Sources
{
    class PaperSourceCIENotes : PaperSource
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

        public override Task<PaperRepository> GetPapersAsync(Subject subject)
        {
            throw new NotImplementedException();
        }

        public override async Task<Dictionary<Subject, string>> GetSubjectUrlMapAsync(Curriculums curriculum)
        {
            throw new NotImplementedException();
        }
    }
}
