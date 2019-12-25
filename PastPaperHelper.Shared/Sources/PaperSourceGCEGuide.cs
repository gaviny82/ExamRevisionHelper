using HtmlAgilityPack;
using PastPaperHelper.Models;
using System.Collections.Generic;
using System.Linq;

namespace PastPaperHelper.Sources
{
    class PaperSourceGCEGuide : PaperSource
    {
        public PaperSourceGCEGuide()
        {
            Name = "GCE Guide";
            Url = "https://papers.gceguide.com/";
        }

        //TODO: scan all papers, then sort
        public override PaperRepository GetPapers(Subject subject, string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url + "/");
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"ggTable\"]/tbody/tr[@class='file']");

            PaperRepository repository = new PaperRepository(subject);
            Dictionary<Exam, List<Paper>> tmpRepo = new Dictionary<Exam, List<Paper>>();

            for (int i = 0; i < nodes.Count; i++)
            {
                string fileName = nodes[i].ChildNodes[1].ChildNodes[0].Attributes["href"].Value;
                string[] split = fileName[0..^4].Split('_');

                if (split.Length > 4 || split.Length < 3 || fileName.Substring(0, 4) != subject.SyllabusCode) continue;

                if (split[1].Length < 3) continue;
                string yr = "20" + split[1].Substring(1, 2);
                ExamYear year = repository.GetExamYear(yr);
                if (year == null)
                {
                    year = new ExamYear { Year = yr };
                    repository.Add(year);
                }

                //Select an exsisting exam or create a new one
                Exam exam;
                switch (split[1][0])
                {
                    default:
                        if (year.Specimen == null)
                        {
                            exam = new Exam
                            {
                                Year = yr,
                                Subject = subject,
                                Series = ExamSeries.Specimen
                            };
                            year.Specimen = exam;
                        }
                        else exam = year.Specimen; break;
                    case 'm':
                        if (year.Spring == null)
                        {
                            exam = new Exam
                            {
                                Year = yr,
                                Subject = subject,
                                Series = ExamSeries.Spring
                            };
                            year.Spring = exam;
                        }
                        else exam = year.Spring; break;
                    case 's':
                        if (year.Summer == null)
                        {
                            exam = new Exam
                            {
                                Year = yr,
                                Subject = subject,
                                Series = ExamSeries.Summer
                            };
                            year.Summer = exam;
                        }
                        else exam = year.Summer; break;
                    case 'w':
                        if (year.Winter == null)
                        {
                            exam = new Exam
                            {
                                Year = yr,
                                Subject = subject,
                                Series = ExamSeries.Winter
                            };
                            year.Winter = exam;
                        }
                        else exam = year.Winter; break;
                }

                if (fileName.Contains("gt"))
                    exam.GradeThreshold = new GradeThreshold { Exam = exam, Url = url + "/" + fileName, };
                else if (fileName.Contains("er"))
                    exam.ExaminersReport = new ExaminersReport { Exam = exam, Url = url + "/" + fileName, };
                else
                {
                    Paper paper = new Paper(fileName, exam, url + "/" + fileName);
                    if (tmpRepo.ContainsKey(exam))
                        tmpRepo[exam].Add(paper);
                    else
                        tmpRepo.Add(exam, new List<Paper> { paper });
                }
            }

            //Sort by components
            foreach (KeyValuePair<Exam, List<Paper>> item in tmpRepo)
            {
                var components = from paper in item.Value
                                 group paper by paper.Component into component
                                 orderby component.Key ascending
                                 select new Component(component.Key, component.ToArray());

                item.Key.Components=components.ToArray();
            }

            repository.Sort();
            return repository;
        }

        public override Dictionary<Subject, string> GetSubjectUrlMap(Curriculums curriculum)
        {
            string url = Url;
            switch (curriculum)
            {
                case Curriculums.IGCSE: url += "IGCSE/"; break;
                case Curriculums.ALevel: url += "A%20Levels/"; break;
            }
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"ggTable\"]/tbody/tr[@class='dir']");

            Dictionary<Subject, string> result = new Dictionary<Subject, string>();
            foreach (HtmlNode node in nodes)
            {
                HtmlNode entry = node.ChildNodes[1].ChildNodes[0];
                HtmlAttribute herf = entry.Attributes["href"];
                string code = entry.InnerText.Split(' ').Last();
                result.Add(new Subject
                {
                    Curriculum = curriculum,
                    Name = entry.InnerText[0..^7],
                    SyllabusCode = code.Substring(1, 4)
                }, url + herf.Value);
            }
            return result;
        }
    }
}
