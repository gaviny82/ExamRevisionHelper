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

        public override PaperRepository GetPapers(SubjectSource subject)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(subject.Url);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"ggTable\"]/tbody/tr[@class='file']");

            Dictionary<(string, ExamSeries), List<PaperItem>> tmpRepo = new Dictionary<(string, ExamSeries), List<PaperItem>>();
            for (int i = 0; i < nodes.Count; i++)
            {
                string file = nodes[i].ChildNodes[1].ChildNodes[0].Attributes["href"].Value;
                string[] split = file.Split('_');

                if (split.Length > 4 || split.Length < 3) continue;

                ExamSeries es;
                switch (split[1][0])
                {
                    default: es = ExamSeries.Spring; break;
                    case 's': es = ExamSeries.Summer; break;
                    case 'w': es = ExamSeries.Winter; break;
                }

                FileTypes t;
                switch (split[2])
                {
                    default: t = FileTypes.Insert; break;
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
                }

                char compCode=' ', varCode=' ';
                if (split.Length > 3) compCode = split[3][0];
                if (split.Length > 3) varCode = split[3][1];

                //Create a new paper
                string yr = "20" + split[1].Substring(1);
                PaperItem paper = new PaperItem
                {
                    //Exames will be assigned later
                    ComponentCode = compCode,
                    VariantCode = varCode,
                    Type = t,
                    Url = subject.Url + "\\" + file,
                };

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

            List<PaperItem> tmpSyllabus = new List<PaperItem>();
            List<Exam> tmpExams = new List<Exam>();
            foreach (KeyValuePair<(string, ExamSeries), List<PaperItem>> item in tmpRepo)
            {
                (string y, ExamSeries e) = item.Key;
                List<PaperItem> lst = item.Value;
                Exam exam = new Exam
                {
                    Subject = subject.SubjectInfo,
                    Year = y,
                    ExamSeries = e
                };
                for (int itor = 0; itor < lst.Count; itor++)
                {
                    PaperItem itm = lst[itor];
                    itm.Exam = exam;
                    if (itm.Type == FileTypes.GradeThreshold)
                    {
                        exam.GradeThreshold = itm;
                        lst.RemoveAt(itor);
                        itor--;
                    }
                    else if (itm.Type==FileTypes.Syllabus)
                    {
                        tmpSyllabus.Add(itm);
                        lst.RemoveAt(itor);
                        itor--;
                    }
                }
                exam.Papers = lst.ToArray();
                tmpExams.Add(exam);
            }

            return new PaperRepository(subject.SubjectInfo)
            {
                Exams = tmpExams.ToArray(),
                Syllabus = tmpSyllabus.ToArray()
            };
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
                string code = entry.InnerText.Split(' ').Last();
                list[i] = new SubjectSource
                {
                    SubjectInfo = new Subject
                    {
                        Curriculum = (Curriculums)curriculum,
                        Name = entry.InnerText.Substring(0, entry.InnerText.Length - 7),
                        SyllabusCode = code.Substring(1, 4),
                    },
                    Url = url + herf.Value
                };
            }
            return list;
        }
    }
}
