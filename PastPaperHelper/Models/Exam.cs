using System.Xml;
using System.Linq;

namespace PastPaperHelper.Models
{
    public enum ExamSeries { Spring, Summer, Winter, Specimen }

    public class Exam
    {
        public Subject Subject { get; set; }
        public string Year { get; set; }
        public ExamSeries Series { get; set; }

        public GradeThreshold GradeThreshold { get; set; }
        public ExaminersReport ExaminersReport { get; set; }
        public Component[] Components { get; set; }

        public Exam() { }

        public Exam(XmlNode node, Subject subject)
        {
            Subject = subject;
            Series = node.Attributes["Series"].Value switch
            {
                "Spring" => ExamSeries.Spring,
                "Summer" => ExamSeries.Summer,
                "Winter" => ExamSeries.Winter,
                _ => ExamSeries.Specimen,
            };
            Year = node.ParentNode.Attributes["Year"].Value;

            if (node.Attributes["GradeThreshold"] != null) GradeThreshold = new GradeThreshold { Exam = this, Url = node.Attributes["GradeThreshold"].Value };
            if (node.Attributes["ExaminersReport"] != null) ExaminersReport = new ExaminersReport { Exam = this, Url = node.Attributes["ExaminersReport"].Value };

            Components = new Component[node.ChildNodes.Count];
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                XmlNode componentNode = node.ChildNodes[i];
                Paper[] plst = new Paper[componentNode.ChildNodes.Count];
                char compCode = char.Parse(componentNode.Attributes["Paper"].Value);

                for (int j = 0; j < componentNode.ChildNodes.Count; j++)
                {
                    XmlNode paperNode = componentNode.ChildNodes[j];
                    plst[j] = new Paper
                    {
                        Exam = this,
                        Component = compCode,
                        Type = (ResourceType)int.Parse(paperNode.Attributes["Type"].Value),
                        Variant = char.Parse(paperNode.Attributes["Variant"].Value),
                        Url = paperNode.Attributes["Url"].Value
                    };
                }
                Components[i] = new Component(compCode, plst);
            }
            var list = from obj in Components orderby obj.Code ascending select obj;
            Components = list.OrderBy(c => c.Code).ToArray();
        }

        public XmlNode GetXmlNode(XmlDocument doc)
        {
            XmlElement examNode = doc.CreateElement("Exam");
            examNode.SetAttribute("Series", Series.ToString());
            if (GradeThreshold != null) examNode.SetAttribute("GradeThreshold", GradeThreshold.Url);
            if (ExaminersReport != null) examNode.SetAttribute("ExaminersReport", ExaminersReport.Url);

            if (Components != null)
            {
                foreach (Component component in Components)
                {
                    XmlElement componentNode = doc.CreateElement("Component");
                    componentNode.SetAttribute("Paper", component.Code.ToString());
                    foreach (Variant vrt in component.Variants)
                    {
                        foreach (Paper paper in vrt.Papers)
                        {
                            XmlElement paperNode = doc.CreateElement("Paper");
                            paperNode.SetAttribute("Url", paper.Url);
                            paperNode.SetAttribute("Variant", paper.Variant.ToString());
                            paperNode.SetAttribute("Type", ((int)paper.Type).ToString());
                            componentNode.AppendChild(paperNode);
                        }
                    }
                    examNode.AppendChild(componentNode);
                }

            }

            return examNode;
        }
    }
}
