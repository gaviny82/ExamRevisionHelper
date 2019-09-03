using System.Xml;
using System.Linq;

namespace PastPaperHelper.Models
{
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
            switch (node.Attributes["Series"].Value)
            {
                default:
                    Series = ExamSeries.Specimen;
                    break;
                case "Spring":
                    Series = ExamSeries.Spring;
                    break;
                case "Summer":
                    Series = ExamSeries.Summer;
                    break;
                case "Winter":
                    Series = ExamSeries.Winter;
                    break;
            }

            if (node.Attributes["GradeThreshold"] != null) GradeThreshold = new GradeThreshold { Exam = this, Url = node.Attributes["GradeThreshold"].Value };
            if (node.Attributes["ExaminersReport"] != null) GradeThreshold = new GradeThreshold { Exam = this, Url = node.Attributes["ExaminersReport"].Value };

            Components = new Component[node.ChildNodes.Count];
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                XmlNode componentNode = node.ChildNodes[i];
                Component component = new Component
                {
                    Code = char.Parse(componentNode.Attributes["Paper"].Value),
                    Papers =new Paper[componentNode.ChildNodes.Count]
                };
                for (int j = 0; j < componentNode.ChildNodes.Count; j++)
                {
                    XmlNode paperNode = componentNode.ChildNodes[j];
                    component.Papers[j] = new Paper
                    {
                        Exam = this,
                        Component = component.Code,
                        Type = (ResourceType)int.Parse(paperNode.Attributes["Type"].Value),
                        Variant = char.Parse(paperNode.Attributes["Variant"].Value),
                        Url = paperNode.Attributes["Url"].Value
                    };
                }
                Components[i] = component;
            }
            var list = from obj in Components orderby obj.Code ascending select obj;
            Components = list.ToArray();
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
                    foreach (Paper paper in component.Papers)
                    {
                        XmlElement paperNode = doc.CreateElement("Paper");
                        paperNode.SetAttribute("Url", paper.Url);
                        paperNode.SetAttribute("Variant", paper.Variant.ToString());
                        paperNode.SetAttribute("Type", ((int)paper.Type).ToString());
                        componentNode.AppendChild(paperNode);
                    }
                    examNode.AppendChild(componentNode);
                }

            }

            return examNode;
        }
    }
}
