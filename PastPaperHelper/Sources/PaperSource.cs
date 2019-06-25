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

        public abstract SubjectSource[] GetSubjects(Curriculums? curriculum = null);
        public abstract PaperItem[] GetPapers(SubjectSource subject);

        public static void SaveSubjectList(SubjectSource[] subjects, string path, string source)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement data = doc.CreateElement("Data");
            data.SetAttribute("Time", DateTime.Now.ToString());
            data.SetAttribute("Source", source);
            doc.AppendChild(data);

            XmlElement IG = doc.CreateElement("IGCSE");
            XmlElement AL = doc.CreateElement("ALevel");
            foreach (SubjectSource subject in subjects)
            {
                XmlElement element = doc.CreateElement("Subject");
                element.SetAttribute("Name", subject.Name);
                element.SetAttribute("SyllabusCode", subject.SyllabusCode);
                element.SetAttribute("Path", subject.Url);
                switch (subject.Curriculum)
                {
                    default: break;
                    case Curriculums.IGCSE: IG.AppendChild(element); break;
                    case Curriculums.ALevel: AL.AppendChild(element); break;
                }
            }
            data.AppendChild(IG);
            data.AppendChild(AL);
            doc.Save(path);
        }
        
        public static void SaveSubscription(Dictionary<SubjectSource, PaperItem[]> list, string path, string source)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement data = doc.CreateElement("Data");
            data.SetAttribute("Time", DateTime.Now.ToString());
            data.SetAttribute("Source", source);
            doc.AppendChild(data);

            foreach(KeyValuePair<SubjectSource,PaperItem[]> item in list)
            {
                SubjectSource subject = item.Key;
                PaperItem[] papers = item.Value;
                XmlElement subj = doc.CreateElement("Subject");
                subj.SetAttribute("Name", subject.Name);
                subj.SetAttribute("SyllabusCode", subject.SyllabusCode);
                data.AppendChild(subj);

                foreach(PaperItem paper in papers)
                {
                    XmlElement pape = doc.CreateElement("Paper");
                    pape.SetAttribute("Path", paper.Url);
                    pape.SetAttribute("Year", paper.Year);
                    pape.SetAttribute("ExamSeries", ((int)paper.ExamSeries).ToString());
                    pape.SetAttribute("Component", paper.ComponentCode.ToString());
                    pape.SetAttribute("Variant", paper.VariantCode.ToString());
                    pape.SetAttribute("Type", ((int)paper.Type).ToString());
                    subj.AppendChild(pape);
                }
            }
            doc.Save(path);
        }
    }

    public static class PaperSources
    {
        public static PaperSource GCE_Guide { get; } = new PaperSourceGCEGuide();
        public static PaperSource PapaCambridge { get; } = new PaperSourcePapaCambridge();
        public static PaperSource CIE_Notes { get; } = new PaperSourceCIENotes();
    }
}
