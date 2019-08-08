using PastPaperHelper.Models;
using System;
using System.Collections.Generic;
using System.Xml;

namespace PastPaperHelper.Sources
{
    public abstract class PaperSource
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public abstract Dictionary<Subject, string> GetSubjectUrlMap(Curriculums? curriculum = null);
        public abstract PaperRepository GetPapers(Subject subject, string url);

        public static void SaveSubjectList(Dictionary<Subject, string> map, XmlDocument doc)
        {
            doc.RemoveAll();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement data = doc.CreateElement("Data");
            data.SetAttribute("Time", DateTime.Now.ToString());
            data.SetAttribute("Source", SubscriptionManager.CurrentPaperSource.Name);
            doc.AppendChild(data);

            XmlElement IG = doc.CreateElement("IGCSE");
            XmlElement AL = doc.CreateElement("ALevel");
            foreach (KeyValuePair<Subject,string> item in map)
            {
                XmlElement element = doc.CreateElement("Subject");
                element.SetAttribute("Name", item.Key.Name);
                element.SetAttribute("SyllabusCode", item.Key.SyllabusCode);
                element.SetAttribute("Url", item.Value);
                switch (item.Key.Curriculum)
                {
                    default: break;
                    case Curriculums.IGCSE: IG.AppendChild(element); break;
                    case Curriculums.ALevel: AL.AppendChild(element); break;
                }
            }
            data.AppendChild(IG);
            data.AppendChild(AL);
        }
        
        public static void SaveSubscription(Dictionary<Subject, PaperRepository> list, XmlDocument doc)
        {
            doc.RemoveAll();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement data = doc.CreateElement("Data");
            data.SetAttribute("Time", DateTime.Now.ToString());
            data.SetAttribute("Source", SubscriptionManager.CurrentPaperSource.Name);
            doc.AppendChild(data);

            foreach(KeyValuePair<Subject, PaperRepository> item in list)
            {
                Subject subject = item.Key;
                PaperRepository repo = item.Value;

                XmlElement subjNode = doc.CreateElement("Subject");
                subjNode.SetAttribute("Name", subject.Name);
                subjNode.SetAttribute("SyllabusCode", subject.SyllabusCode);
                data.AppendChild(subjNode);

                foreach (Syllabus sy in repo.Syllabus)
                {
                    XmlElement syl = doc.CreateElement("Syllabus");
                    syl.SetAttribute("Year", sy.Year);
                    syl.SetAttribute("Url", sy.Url);
                    subjNode.AppendChild(syl);
                }

                foreach (Exam exam in repo.Exams)
                {
                    XmlElement series = doc.CreateElement("ExamSeries");
                    series.SetAttribute("Year", exam.Year);
                    series.SetAttribute("Series", ((int)exam.Series).ToString());
                    if (exam.GradeThreshold != null)
                    {
                        XmlElement gt = doc.CreateElement("GradeThreshold");
                        gt.SetAttribute("Url", exam.GradeThreshold.Url);
                        series.AppendChild(gt);
                    }
                    foreach (Paper paper in exam.Papers)
                    {
                        XmlElement paperNode = doc.CreateElement("Paper");
                        paperNode.SetAttribute("Url", paper.Url);
                        paperNode.SetAttribute("Component", paper.ComponentCode.ToString());
                        paperNode.SetAttribute("Variant", paper.VariantCode.ToString());
                        paperNode.SetAttribute("Type", ((int)paper.Type).ToString());
                        series.AppendChild(paperNode);
                    }
                    subjNode.AppendChild(series);
                }
            }
        }
    }

    public static class PaperSources
    {
        public static PaperSource GCE_Guide { get; } = new PaperSourceGCEGuide();
        public static PaperSource PapaCambridge { get; } //= new PaperSourcePapaCambridge();
        public static PaperSource CIE_Notes { get; } //= new PaperSourceCIENotes();
    }
}
