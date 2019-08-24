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

            Dictionary<Exam, List<Paper>> tmpRepo = new Dictionary<Exam, List<Paper>>();
            List<Syllabus> tmpSyllabus = new List<Syllabus>();

            for (int i = 0; i < nodes.Count; i++)
            {
                string file = nodes[i].ChildNodes[1].ChildNodes[0].Attributes["href"].Value;
                string[] split = file.Substring(0, file.Length - 4).Split('_');

                if (split.Length > 4 || split.Length < 3) continue;

                string yr = "20" + split[1].Substring(1);

                ExamSeries es;
                switch (split[1][0])
                {
                    default: es = ExamSeries.Specimen; break;
                    case 'm': es = ExamSeries.Spring; break;
                    case 's': es = ExamSeries.Summer; break;
                    case 'w': es = ExamSeries.Winter; break;
                }

                Exam exam = null;
                foreach (KeyValuePair<Exam, List<Paper>> item in tmpRepo)
                {
                    if (item.Key.Year == yr && item.Key.Series == es)
                    {
                        exam = item.Key;
                        break;
                    }
                }
                if (exam == null)
                {
                    exam = new Exam
                    {
                        Subject = subject,
                        Year = yr,
                        Series = es
                    };
                    tmpRepo.Add(exam, new List<Paper>());
                }

                FileTypes t;
                switch (split[2])
                {
                    default: t = FileTypes.Unknown; break;
                    case "ir": t = FileTypes.ConfidentialInstructions; break;
                    case "ci": t = FileTypes.ConfidentialInstructions; break;
                    case "su": t = FileTypes.ListeningAudio; break;
                    case "sf": t = FileTypes.ListeningAudio; break;
                    case "ms": t = FileTypes.MarkScheme; break;
                    case "qp": t = FileTypes.QuestionPaper; break;
                    case "rp": t = FileTypes.SpeakingTestCards; break;

                    case "sy":
                        tmpSyllabus.Add(new Syllabus
                        {
                            Url = url + "/" + file,
                            Year = yr
                        });
                        continue;

                    case "gt":
                        exam.GradeThreshold = new GradeThreshold
                        {
                            Exam = exam,
                            Url = url + "/" + file,
                        };
                        continue;

                    case "er":
                        exam.ExaminersReport = new ExaminersReport
                        {
                            Exam = exam,
                            Url = url + "/" + file,
                        };
                        continue;

                    case "tn": t = FileTypes.TeachersNotes; break;
                    case "qr": t = FileTypes.Transcript; break;
                    case "in": t = FileTypes.Insert; break;
                    case "in2": t = FileTypes.Insert; break;
                    case "i2": t = FileTypes.Insert; break;
                }

                char compCode=' ', varCode=' ';
                if (split.Length > 3)
                {
                    compCode = split[3][0];
                    if (split[3].Length > 1) varCode = split[3][1];
                }

                //Create a new paper and add to temporary list
                tmpRepo[exam].Add(new Paper
                {
                    Exam = exam,
                    Component = compCode,
                    Variant = varCode,
                    Type = t,
                    Url = url + "/" + file,
                });
            }
            foreach (KeyValuePair<Exam, List<Paper>> item in tmpRepo)
            {
                item.Key.Papers = item.Value.ToArray();
            }

            return new PaperRepository(subject)
            {
                Exams = tmpRepo.Keys.ToArray(),
                Syllabus = tmpSyllabus.ToArray()
            };
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
                    Curriculum = (Curriculums)curriculum,
                    Name = entry.InnerText.Substring(0, entry.InnerText.Length - 7),
                    SyllabusCode = code.Substring(1, 4)
                }, url + herf.Value);
            }
            return result;
        }
    }
}
