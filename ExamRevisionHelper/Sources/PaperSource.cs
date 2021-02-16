using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using ExamRevisionHelper.Models;
using HtmlAgilityPack;

namespace ExamRevisionHelper.Sources
{
    public abstract class PaperSource
    {

        protected static HtmlWeb web = new HtmlWeb();
        public string Name { get; protected set; }
        public string DisplayName { get; protected set; }

        public string UrlBase { get; protected set; }
        public Dictionary<Subject, string> SubjectUrlMap { get; private set; } = new Dictionary<Subject, string>();
        public Dictionary<Subject, PaperRepository> Subscription { get; private set; } = new Dictionary<Subject, PaperRepository>();
        public DateTime LastUpdated { get; private set; }

        public PaperSource()
        {

        }
        public PaperSource(XmlDocument data)
        {
            XmlNode dataNode = data.SelectSingleNode("/Data");
            if (dataNode == null || dataNode.Attributes["LastUpdate"] == null) throw new Exception("Failed to load source data.");

            //Load time of last update
            DateTime.TryParse(dataNode.Attributes["LastUpdate"].Value, out DateTime lastUpdate);
            LastUpdated = lastUpdate;
        }

        public async virtual Task UpdateSubjectUrlMapAsync()
        {
            Task<Dictionary<Subject, string>> repoIG = null;
            Task<Dictionary<Subject, string>> repoAL = null;
            try
            {
                repoIG = GetSubjectUrlMapAsync(Curriculums.IGCSE);
                repoAL = GetSubjectUrlMapAsync(Curriculums.ALevel);
            }
            catch (Exception e)
            {
                //Once exception arised, stop updating
                throw e;
            }
            Dictionary<Subject, string> tmp = new Dictionary<Subject, string>();
            foreach (var item in await repoIG) tmp.Add(item.Key, item.Value);
            foreach (var item in await repoAL) tmp.Add(item.Key, item.Value);
            SubjectUrlMap = tmp;
        }

        public async virtual Task AddOrUpdateSubject(Subject subj)
        {
            if (!SubjectUrlMap.ContainsKey(subj)) throw new Exception($"Cannot find {subj.SyllabusCode} {subj.Name} in {nameof(SubjectUrlMap)}");
            var repo = await GetPapers(subj);
            if (Subscription.ContainsKey(subj)) Subscription[subj] = repo;
            else Subscription.Add(subj, repo);
        }

        public abstract Task<Dictionary<Subject, string>> GetSubjectUrlMapAsync(Curriculums curriculum);

        public abstract Task<PaperRepository> GetPapers(Subject subject);

        public XmlDocument SaveDataToXml(Dictionary<Subject, PaperRepository> subscription = null)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));

            //Write source info
            XmlElement dataNode = doc.CreateElement("Data");
            dataNode.SetAttribute("LastUpdate", DateTime.Now.ToString());
            dataNode.SetAttribute("Source", Name);
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

            if (subscription == null) subscription = Subscription;
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
