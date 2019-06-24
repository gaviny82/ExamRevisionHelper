using PastPaperHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PastPaperHelper.Sources
{
    public abstract class PaperSource
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public abstract Subject[] GetSubjects(Curriculums? curriculum = null);
        public abstract PaperItem[] GetPapers(Subject subject);

        public static void SaveSubjectList(Subject[] subjects, string path, string source)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement data = doc.CreateElement("Data");
            data.SetAttribute("Time", DateTime.Now.ToString());
            data.SetAttribute("Source", source);
            doc.AppendChild(data);

            XmlElement IG = doc.CreateElement("IGCSE");
            XmlElement AL = doc.CreateElement("ALevel");
            foreach (Subject subject in subjects)
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
            doc.Save(Environment.CurrentDirectory + "\\subject_list.xml");
        }
        
        public static void SavePaperList(PaperItem[] papers, string path)
        {
            
        }
    }

    public static class PaperSources
    {
        public static PaperSource GCE_Guide { get; } = new PaperSourceGCEGuide();
        public static PaperSource PapaCambridge { get; } = new PaperSourcePapaCambridge();
        public static PaperSource CIE_Notes { get; } = new PaperSourceCIENotes();
    }
}
