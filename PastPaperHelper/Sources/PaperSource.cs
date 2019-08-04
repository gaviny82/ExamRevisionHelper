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

                XmlElement subj = doc.CreateElement("Subject");
                subj.SetAttribute("Name", subject.Name);
                subj.SetAttribute("SyllabusCode", subject.SyllabusCode);
                data.AppendChild(subj);

                foreach (PaperItem sy in repo.Syllabus)
                {
                    XmlElement syl = doc.CreateElement("Syllabus");
                    syl.SetAttribute("Year", sy.Exam.Year);
                    syl.SetAttribute("Url", sy.Url);
                    subj.AppendChild(syl);
                }

                foreach (Exam exam in repo.Exams)
                {
                    XmlElement series = doc.CreateElement("ExamSeries");
                    series.SetAttribute("Year", exam.Year);
                    series.SetAttribute("Series", ((int)exam.ExamSeries).ToString());
                    foreach (PaperItem paper in exam.Papers)
                    {
                        XmlElement pap = doc.CreateElement("Paper");
                        pap.SetAttribute("Url", paper.Url);
                        pap.SetAttribute("Component", paper.ComponentCode.ToString());
                        pap.SetAttribute("Variant", paper.VariantCode.ToString());
                        pap.SetAttribute("Type", ((int)paper.Type).ToString());
                        series.AppendChild(pap);
                    }
                    subj.AppendChild(series);
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
