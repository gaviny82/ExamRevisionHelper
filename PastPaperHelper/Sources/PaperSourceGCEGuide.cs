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

        public override PaperRepository GetPapers(Subject subject, string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(SubscriptionManager.SubjectUrlMap[subject]);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"ggTable\"]/tbody/tr[@class='file']");

            PaperRepository repository = new PaperRepository(subject);
            Dictionary<Exam, List<Paper>> tmpRepo = new Dictionary<Exam, List<Paper>>();

            for (int i = 0; i < nodes.Count; i++)
            {
                string file = nodes[i].ChildNodes[1].ChildNodes[0].Attributes["href"].Value;
                string[] split = file.Substring(0, file.Length - 4).Split('_');

                if (split.Length > 4 || split.Length < 3 || file.Substring(0, 4) != subject.SyllabusCode) continue;

                if (split[1].Length < 3) continue;
                string yr = "20" + split[1].Substring(1, 2);
                ExamYear year = repository.GetExamYear(yr);
                if (year == null)
                {
                    year = new ExamYear { Year = yr };
                    repository.Add(year);
                }

                ResourceType t;
                switch (split[2])
                {
                    default: t = ResourceType.Unknown; break;
                    case "ir": t = ResourceType.ConfidentialInstructions; break;
                    case "ci": t = ResourceType.ConfidentialInstructions; break;
                    case "su": t = ResourceType.ListeningAudio; break;
                    case "sf": t = ResourceType.ListeningAudio; break;
                    case "ms": t = ResourceType.MarkScheme; break;
                    case "qp": t = ResourceType.QuestionPaper; break;
                    case "rp": t = ResourceType.SpeakingTestCards; break;

                    case "sy":
                        year.Syllabus = new Syllabus
                        {
                            Url = url + "/" + file,
                            Year = yr
                        };
                        continue;

                    case "gt": t = ResourceType.GradeThreshold; break;
                    case "er": t = ResourceType.ExaminersReport; break;
                    case "tn": t = ResourceType.TeachersNotes; break;
                    case "qr": t = ResourceType.Transcript; break;
                    case "in": t = ResourceType.Insert; break;
                    case "in2": t = ResourceType.Insert; break;
                    case "i2": t = ResourceType.Insert; break;
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

                //Create er/gt after exam is selected
                if (t == ResourceType.ExaminersReport)
                {
                    exam.ExaminersReport = new ExaminersReport
                    {
                        Exam = exam,
                        Url = url + "/" + file,
                    };
                    continue;
                }
                else if( t== ResourceType.GradeThreshold)
                {
                    exam.GradeThreshold = new GradeThreshold
                    {
                        Exam = exam,
                        Url = url + "/" + file,
                    };
                    continue;
                }


                char compCode=' ', varCode=' ';
                if (split.Length > 3)
                {
                    compCode = split[3][0];
                    if (split[3].Length > 1) varCode = split[3][1];
                }

                //Create a new paper and add to temporary list
                Paper paper = new Paper
                {
                    Exam = exam,
                    Component = compCode,
                    Variant = varCode,
                    Type = t,
                    Url = url + "/" + file,
                };
                if (tmpRepo.ContainsKey(exam))
                    tmpRepo[exam].Add(paper);
                else
                    tmpRepo.Add(exam, new List<Paper> { paper });
            }


            foreach (KeyValuePair<Exam, List<Paper>> item in tmpRepo)
            {
                //Sort by components
                Exam exam = item.Key;
                Dictionary<char, List<Paper>> components = new Dictionary<char, List<Paper>>();
                foreach (Paper paper in item.Value)
                {
                    if (!components.ContainsKey(paper.Component))
                        components.Add(paper.Component, new List<Paper> { paper });
                    else
                        components[paper.Component].Add(paper);
                }

                exam.Components = new Component[components.Count];
                int i = 0;

                var sortedComponents = from objDic in components orderby objDic.Key ascending select objDic;
                foreach (KeyValuePair<char, List<Paper>> component in sortedComponents)
                {
                    exam.Components[i++] = new Component
                    (
                        component.Key,
                        component.Value.ToArray()
                    );
                }
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
                    Name = entry.InnerText.Substring(0, entry.InnerText.Length - 7),
                    SyllabusCode = code.Substring(1, 4)
                }, url + herf.Value);
            }
            return result;
        }
    }
}
