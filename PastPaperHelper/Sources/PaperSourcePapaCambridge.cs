using HtmlAgilityPack;
using PastPaperHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.Sources
{
    class PaperSourcePapaCambridge : PaperSource
    {
        public PaperSourcePapaCambridge()
        {
            Name = "PapaCambridge";
            Url = "https://papers.gceguide.com/";
        }

        public override PaperItem[] GetPapers(Subject subject)
        {
            throw new NotImplementedException();
        }

        public override Subject[] GetSubjects(Curriculums? curriculum = null)
        {
            if (curriculum == null)
            {
                Subject[] IG = GetSubjects(Curriculums.IGCSE);
                Subject[] AL = GetSubjects(Curriculums.ALevel);

                Subject[] result = new Subject[IG.Length + AL.Length];
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

            Subject[] list = new Subject[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                HtmlNode entry = nodes[i].ChildNodes[1].ChildNodes[0];
                HtmlAttribute herf = entry.Attributes["href"];
                string name = entry.InnerText.Split(' ')[0];
                list[i] = new Subject
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
