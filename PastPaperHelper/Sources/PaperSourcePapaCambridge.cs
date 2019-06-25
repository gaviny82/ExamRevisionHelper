using HtmlAgilityPack;
using PastPaperHelper.Models;
using System;

namespace PastPaperHelper.Sources
{
    class PaperSourcePapaCambridge : PaperSource
    {
        public PaperSourcePapaCambridge()
        {
            Name = "PapaCambridge";
            Url = "https://papers.gceguide.com/";
        }

        public override PaperItem[] GetPapers(SubjectSource subject)
        {
            throw new NotImplementedException();
        }

        public override SubjectSource[] GetSubjects(Curriculums? curriculum = null)
        {
            if (curriculum == null)
            {
                SubjectSource[] IG = GetSubjects(Curriculums.IGCSE);
                SubjectSource[] AL = GetSubjects(Curriculums.ALevel);

                SubjectSource[] result = new SubjectSource[IG.Length + AL.Length];
                for (int i = 0; i < IG.Length; i++) result[i] = IG[i];
                for (int i = 0; i < AL.Length; i++) result[i + IG.Length] = AL[i];

                return result;
            }
            string url = Url;
            switch (curriculum)
            {
                case Curriculums.IGCSE: url += "IGCSE/"; break;
                case Curriculums.ALevel: url += "A%20Levels/"; break;
            }
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"ggTable\"]/tbody/tr[@class='dir']");

            SubjectSource[] list = new SubjectSource[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                HtmlNode entry = nodes[i].ChildNodes[1].ChildNodes[0];
                HtmlAttribute herf = entry.Attributes["href"];
                string name = entry.InnerText.Split(' ')[0];
                list[i] = new SubjectSource
                {
                    Curriculum = (Curriculums)curriculum,
                    Name = name,
                    SyllabusCode = entry.InnerText.Substring(name.Length + 1, 4),
                    Url = herf.Value
                };
            }
            return list;
        }
    }
}
