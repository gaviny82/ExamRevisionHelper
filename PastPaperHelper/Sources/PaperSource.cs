using PastPaperHelper.Core.Tools;
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

        public virtual Dictionary<Subject, string> GetSubjectUrlMap()
        {
            Dictionary<Subject, string> repoIG = GetSubjectUrlMap(Curriculums.IGCSE);
            Dictionary<Subject, string> repoAL = GetSubjectUrlMap(Curriculums.ALevel);

            Dictionary<Subject, string> tmp = new Dictionary<Subject, string>();
            foreach (KeyValuePair<Subject, string> item in repoIG) tmp.Add(item.Key, item.Value);
            foreach (KeyValuePair<Subject, string> item in repoAL) tmp.Add(item.Key, item.Value);
            return tmp;
        }

        public abstract Dictionary<Subject, string> GetSubjectUrlMap(Curriculums curriculum);

        public abstract PaperRepository GetPapers(Subject subject, string url);

        public static void SaveSubjectList(Dictionary<Subject, string> map, XmlDocument doc)
        {
            doc.RemoveAll();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement data = doc.CreateElement("Data");
            data.SetAttribute("Time", DateTime.Now.ToString());
            data.SetAttribute("Source", PastPaperHelperCore.CurrentSource.Name);
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
            data.SetAttribute("Source", PastPaperHelperCore.CurrentSource.Name);
            doc.AppendChild(data);

            foreach(KeyValuePair<Subject, PaperRepository> item in list)
            {
                Subject subject = item.Key;
                PaperRepository repo = item.Value;

                XmlElement subjNode = doc.CreateElement("Subject");
                subjNode.SetAttribute("Name", subject.Name);
                subjNode.SetAttribute("SyllabusCode", subject.SyllabusCode);
                data.AppendChild(subjNode);

                foreach (ExamYear year in repo)
                {
                    XmlElement yearNode = doc.CreateElement("ExamYear");
                    yearNode.SetAttribute("Year", year.Year);
                    if (year.Syllabus != null)  yearNode.SetAttribute("Syllabus", year.Syllabus.Url);
                    if (year.Specimen != null) yearNode.AppendChild(year.Specimen.GetXmlNode(doc));
                    if (year.Spring != null) yearNode.AppendChild(year.Spring.GetXmlNode(doc));
                    if (year.Summer != null) yearNode.AppendChild(year.Summer.GetXmlNode(doc));
                    if (year.Winter != null) yearNode.AppendChild(year.Winter.GetXmlNode(doc));
                    subjNode.AppendChild(yearNode);
                }
            }
            doc.Save(Environment.CurrentDirectory + "\\data\\subscription.xml");
        }

        public void Save(Dictionary<Subject, string> map, Dictionary<Subject, PaperRepository> list, XmlDocument doc)
        {
            doc.RemoveAll();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));

            //Write update info
            XmlElement data = doc.CreateElement("Data");
            data.SetAttribute("LastUpdate", DateTime.Now.ToString());
            data.SetAttribute("Source", Name);
            doc.AppendChild(data);

            XmlElement subjListNode = doc.CreateElement("SubjectList");
            data.AppendChild(subjListNode);

            XmlElement IG = doc.CreateElement("IGCSE");
            XmlElement AL = doc.CreateElement("ALevel");
            foreach (KeyValuePair<Subject, string> item in map)
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
            subjListNode.AppendChild(IG);
            subjListNode.AppendChild(AL);

            //Write subscription data
            XmlElement subscriptionNode = doc.CreateElement("Subscription");
            data.AppendChild(subscriptionNode);

            foreach (KeyValuePair<Subject, PaperRepository> item in list)
            {
                Subject subject = item.Key;
                PaperRepository repo = item.Value;

                XmlElement subjNode = doc.CreateElement("Subject");
                subjNode.SetAttribute("Name", subject.Name);
                subjNode.SetAttribute("SyllabusCode", subject.SyllabusCode);
                subscriptionNode.AppendChild(subjNode);

                foreach (ExamYear year in repo)
                {
                    XmlElement yearNode = doc.CreateElement("ExamYear");
                    yearNode.SetAttribute("Year", year.Year);
                    if (year.Syllabus != null) yearNode.SetAttribute("Syllabus", year.Syllabus.Url);
                    if (year.Specimen != null) yearNode.AppendChild(year.Specimen.GetXmlNode(doc));
                    if (year.Spring != null) yearNode.AppendChild(year.Spring.GetXmlNode(doc));
                    if (year.Summer != null) yearNode.AppendChild(year.Summer.GetXmlNode(doc));
                    if (year.Winter != null) yearNode.AppendChild(year.Winter.GetXmlNode(doc));
                    subjNode.AppendChild(yearNode);
                }
            }

        }
    }

    public static class PaperSources
    {
        public static PaperSource GCE_Guide { get; } = new PaperSourceGCEGuide();
        public static PaperSource PapaCambridge { get; }// = new PaperSourcePapaCambridge();
        public static PaperSource CIE_Notes { get; } //= new PaperSourceCIENotes();
    }
}
