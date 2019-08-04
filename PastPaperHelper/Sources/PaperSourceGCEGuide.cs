using HtmlAgilityPack;
using PastPaperHelper.Models;
using System;
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

            Dictionary<(string, ExamSeries), List<PaperItem>> tmpRepo = new Dictionary<(string, ExamSeries), List<PaperItem>>();
            List<PaperItem> tmpSyllabus = new List<PaperItem>();
            List<Exam> tmpExams = new List<Exam>();

            for (int i = 0; i < nodes.Count; i++)
            {
                string file = nodes[i].ChildNodes[1].ChildNodes[0].Attributes["href"].Value;
                string[] split = file.Substring(0, file.Length - 4).Split('_');

                if (split.Length > 4 || split.Length < 3) continue;

                ExamSeries es;
                switch (split[1][0])
                {
                    default: es = ExamSeries.Specimen; break;
                    case 'm': es = ExamSeries.Spring; break;
                    case 's': es = ExamSeries.Summer; break;
                    case 'w': es = ExamSeries.Winter; break;
                }

                FileTypes t;
                switch (split[2])
                {
                    default: t = FileTypes.Unknown; break;
                    case "er": t = FileTypes.ExaminersReport; break;
                    case "gt": t = FileTypes.GradeThreshold; break;
                    case "su": t = FileTypes.ListeningAudio; break;
                    case "sf": t = FileTypes.ListeningAudio; break;
                    case "ms": t = FileTypes.MarkScheme; break;
                    case "qp": t = FileTypes.QuestionPaper; break;
                    case "rp": t = FileTypes.SpeakingTestCards; break;
                    case "sy": t = FileTypes.Syllabus; break;
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

                //Create a new paper
                string yr = "20" + split[1].Substring(1);
                PaperItem paper = new PaperItem
                {
                    //Exames will be assigned later
                    ComponentCode = compCode,
                    VariantCode = varCode,
                    Type = t,
                    Url = url + file,
                };

                if (paper.Type == FileTypes.Syllabus)
                {
                    paper.Exam = new Exam { Year = yr };
                    tmpSyllabus.Add(paper);
                    continue;
                }
                //Add to temporary list
                if (tmpRepo.ContainsKey((yr, es)))
                {
                    tmpRepo[(yr, es)].Add(paper);
                }
                else
                {
                    tmpRepo.Add((yr, es), new List<PaperItem> { paper });
                }
            }

            foreach (KeyValuePair<(string, ExamSeries), List<PaperItem>> item in tmpRepo)
            {
                (string y, ExamSeries e) = item.Key;
                List<PaperItem> lst = item.Value;
                Exam exam = new Exam
                {
                    Subject = subject,
                    Year = y,
                    ExamSeries = e
                };
                for (int itor = 0; itor < lst.Count; itor++)
                {
                    PaperItem itm = lst[itor];
                    itm.Exam = exam;
                }
                exam.Papers = lst.ToArray();
                tmpExams.Add(exam);
            }

            return new PaperRepository(subject)
            {
                Exams = tmpExams.ToArray(),
                Syllabus = tmpSyllabus.ToArray()
            };
        }

        public override Dictionary<Subject, string> GetSubjectUrlMap(Curriculums? curriculum = null)
        {
            if (curriculum == null)
            {
                Dictionary<Subject, string> tmp = new Dictionary<Subject, string>();
                foreach (KeyValuePair<Subject, string> item in GetSubjectUrlMap(Curriculums.IGCSE)) tmp.Add(item.Key, item.Value);
                foreach (KeyValuePair<Subject, string> item in GetSubjectUrlMap(Curriculums.ALevel)) tmp.Add(item.Key, item.Value);
                return tmp;
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
