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
            HtmlNodeCollection examNodes = doc.DocumentNode.SelectNodes("//table[1]//td[@data-name and @data-href]");

            string resUrl1 = "", resUrl2 = "";
            List<Exam> examList = new List<Exam>();
            foreach (HtmlNode examNode in examNodes)
            {
                string examUrl = "https://pastpapers.papacambridge.com/" + examNode.Attributes["data-href"].Value;
                string examCode = examNode.Attributes["data-name"].Value; ;

                if (examCode.Contains("&"))
                {
                    if (string.IsNullOrEmpty(resUrl1)) resUrl1 = examUrl;
                    else resUrl2 = examUrl;
                    continue;
                }

                string substr = examCode.Substring(5);

                ExamSeries series;
                if (substr.Contains("Mar")) series = ExamSeries.Spring;
                else if (substr.Contains("Jun")) series = ExamSeries.Summer;
                else series = ExamSeries.Winter;

                Exam exam = new Exam
                {
                    Subject = subject,
                    Year = examCode.Substring(0, 4),
                    Series = series,
                };

                HtmlDocument examPage = web.Load(examUrl);
                HtmlNodeCollection paperNodes = examPage.DocumentNode.SelectNodes("//table[1]/tbody//td[@data-name!=\"..\"]");

                List<Paper> paperList = new List<Paper>();
                foreach (HtmlNode paperNode in paperNodes)
                {
                    string fileName = paperNode.Attributes["data-name"].Value;
                    string fileUrl = "https://pastpapers.papacambridge.com/" + paperNode.Attributes["data-href"].Value;
                    if (fileName.Contains("gt"))
                        exam.GradeThreshold = new GradeThreshold { Exam = exam, Url = fileUrl };
                    else if(fileName.Contains("er"))
                        exam.ExaminersReport = new ExaminersReport { Exam = exam, Url = fileUrl };//Not available
                    else
                        paperList.Add(new Paper(fileName, exam, fileUrl));
                }
                exam.Papers = paperList.ToArray();
                examList.Add(exam);
            }
            //read specimen papers and syllabus

            repo.Exams = examList.ToArray();
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
