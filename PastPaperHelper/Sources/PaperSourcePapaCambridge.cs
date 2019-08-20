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
            Name = "PapaCambridge";
            Url = "https://papacambridge.com/";
        }

        public override PaperRepository GetPapers(Subject subject, string url)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<Subject, string> GetSubjectUrlMap(Curriculums curriculum)
        {
            string url = Url;
            switch (curriculum)
            {
                case Curriculums.IGCSE: url += "igcse-subjects/"; break;
                case Curriculums.ALevel: url += "a-and-as-level-subjects/"; break;
            }
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("/html/body/div[1]/div/p");

            Dictionary<Subject, string> map = new Dictionary<Subject, string>();

            foreach (HtmlNode node in nodes)
            {
                HtmlNode nameTag = node.ChildNodes.FindFirst("a");
                HtmlNode syCodeTag = node.ChildNodes.FindFirst("span");
                Subject subj = new Subject
                {
                    Curriculum = curriculum,
                    Name = nameTag.InnerText,
                    SyllabusCode = syCodeTag.InnerHtml.Substring(1, 4)
                };
                map.Add(subj, nameTag.Attributes[0].Value);
            }

            return map;
        }
    }
}
