using PastPaperHelper.Core.Tools;
using PastPaperHelper.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace PastPaperHelper.Sources
{
    public abstract class PaperSource
    {
        public string Name { get; set; }
        public string UrlBase { get; set; }
        public Dictionary<Subject, string> SubjectUrlMap { get; private set; } = new Dictionary<Subject, string>();
        public Dictionary<Subject, PaperRepository> Subscription { get; set; } = new Dictionary<Subject, PaperRepository>();
        public DateTime LastUpdated { get; private set; }

        public PaperSource(XmlDocument data)
        {

        }

        public async virtual Task UpdateSubjectUrlMapAsync()
        {
            var repoIG = GetSubjectUrlMapAsync(Curriculums.IGCSE);
            var repoAL = GetSubjectUrlMapAsync(Curriculums.ALevel);
            
            Dictionary<Subject, string> tmp = new Dictionary<Subject, string>();
            foreach (var item in await repoIG) tmp.Add(item.Key, item.Value);
            foreach (var item in await repoAL) tmp.Add(item.Key, item.Value);

            SubjectUrlMap = tmp;
        }

        public abstract Task<Dictionary<Subject, string>> GetSubjectUrlMapAsync(Curriculums curriculum);

        public abstract Task<PaperRepository> GetPapers(Subject subject);

        public XmlDocument SaveDataToXml(Dictionary<Subject, PaperRepository> subscription)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));

            //Write source info
            XmlElement dataNode = doc.CreateElement("Data");
            dataNode.SetAttribute("Time", DateTime.Now.ToString());
            dataNode.SetAttribute("Source", PastPaperHelperCore.Source.Name);
            doc.AppendChild(dataNode);

            //Write subject and url pairs
            XmlElement subjList = doc.CreateElement("SubjectList");
            dataNode.AppendChild(subjList);
            XmlElement igSubjNode = doc.CreateElement("IGCSE");
            XmlElement alSubjNode = doc.CreateElement("ALevel");
            subjList.AppendChild(igSubjNode);
            subjList.AppendChild(alSubjNode);

            foreach (KeyValuePair<Subject, string> item in SubjectUrlMap)
            {
                XmlElement element = doc.CreateElement("Subject");
                element.SetAttribute("Name", item.Key.Name);
                element.SetAttribute("SyllabusCode", item.Key.SyllabusCode);
                element.SetAttribute("Url", item.Value);
                switch (item.Key.Curriculum)
                {
                    default: break;
                    case Curriculums.IGCSE: igSubjNode.AppendChild(element); break;
                    case Curriculums.ALevel: alSubjNode.AppendChild(element); break;
                }
            }

            //Write subscribed subjects and exams
            XmlElement subsNode = doc.CreateElement("Subscription");
            dataNode.AppendChild(subsNode);

            if (subscription == null) return doc;
            foreach (KeyValuePair<Subject, PaperRepository> item in subscription)
            {
                Subject subject = item.Key;
                PaperRepository repo = item.Value;

                XmlElement subjNode = doc.CreateElement("Subject");
                subjNode.SetAttribute("Name", subject.Name);
                subjNode.SetAttribute("SyllabusCode", subject.SyllabusCode);
                subsNode.AppendChild(subjNode);

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
            return doc;
        }
    }
}
