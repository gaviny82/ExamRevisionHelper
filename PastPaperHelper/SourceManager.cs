using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PastPaperHelper
{
    public enum PastPaperSources { GCEGuide, PapaCambridge, CIEnotes }
    public static class SourceManager
    {
        public static XmlDocument UpdateSubjectsFromSource(PastPaperSources source)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement data = doc.CreateElement("Data");
            data.SetAttribute("Time", DateTime.Now.ToString());
            doc.AppendChild(data);

            if (source == PastPaperSources.GCEGuide)
            {
                HtmlWeb web = new HtmlWeb();
                string url = "";
                switch (source)
                {
                    default: return null;
                    case PastPaperSources.GCEGuide: url = @"https://papers.gceguide.com/"; break;
                }

                data.SetAttribute("Source", url);
                XmlElement IGCSE = doc.CreateElement("IGCSE");
                HtmlDocument doc1 = web.Load(url + "IGCSE/");
                HtmlNodeCollection igcse = doc1.DocumentNode.SelectNodes("//*[@id=\"ggTable\"]/tbody/tr[@class='dir']");
                foreach (HtmlNode node in igcse)
                {
                    HtmlNode entry = node.ChildNodes[1].ChildNodes[0];
                    HtmlAttribute herf = entry.Attributes["href"];
                    XmlElement element = doc.CreateElement("Subject");
                    element.SetAttribute("Name", entry.InnerText);
                    element.SetAttribute("Path", "IGCSE/" + herf.Value);
                    IGCSE.AppendChild(element);
                }
                data.AppendChild(IGCSE);


                XmlElement AL = doc.CreateElement("ALevel");
                HtmlDocument doc2 = web.Load(url + "A%20Levels/");
                HtmlNodeCollection alevel = doc2.DocumentNode.SelectNodes("//*[@id=\"ggTable\"]/tbody/tr[@class='dir']");
                foreach (HtmlNode node in alevel)
                {
                    HtmlNode entry = node.ChildNodes[1].ChildNodes[0];
                    HtmlAttribute herf = entry.Attributes["href"];
                    XmlElement element = doc.CreateElement("Subject");
                    element.SetAttribute("Name", entry.InnerText);
                    element.SetAttribute("Path", "A%20Level/" + herf.Value);
                    AL.AppendChild(element);
                }
                data.AppendChild(AL);
                doc.Save(Environment.CurrentDirectory + "\\subject_list.xml");
            }
            return doc;
        }
    }
}
