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
            PaperRepository repo = new PaperRepository(subject);
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            HtmlNodeCollection examNodes = doc.DocumentNode.SelectNodes("/html/body/section[2]/div/div/div[1]/div[1]/div/div[2]/div/div/section/div/div/div/div/div/div/div/div/table/tbody/tr[@class=\"point\"]");

            string resUrl1 = "", resUrl2 = "";
            List<Exam> examList = new List<Exam>();
            foreach (HtmlNode examNode in examNodes)
            {
                HtmlNode linkNode = examNode.ChildNodes.FindFirst("td").ChildNodes.FindFirst("a");
                string examUrl = linkNode.Attributes["href"].Value;
                HtmlNode textNode = linkNode.ChildNodes.FindFirst("label");
                string label = textNode.InnerText;

                if (label.Contains("&"))
                {
                    if (string.IsNullOrEmpty(resUrl1)) resUrl1 = examUrl;
                    else resUrl2 = examUrl;
                    continue;
                }

                string substr = label.Substring(5);

                ExamSeries series;
                if (substr.Contains("Mar")) series = ExamSeries.Spring;
                else if (substr.Contains("Jun")) series = ExamSeries.Summer;
                else series = ExamSeries.Winter;

                Exam exam = new Exam
                {
                    Subject = subject,
                    Year = label.Substring(0, 4),
                    Series = series,
                };

                HtmlDocument examPage = web.Load(examUrl);
                HtmlNodeCollection paperNodes = examPage.DocumentNode.SelectNodes("//*[@id=\"data\"]/div/div/div[1]/div/table/tbody//td[@data-name!=\"..\"]");

                examList.Add(exam);
            }
            //read specimen papers and syllabus

            return repo;
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
