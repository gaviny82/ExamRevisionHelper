using System.Xml;

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

        public Exam(XmlNode node)
        {
            throw new System.Exception();
        }

        public XmlNode GetXmlNode(XmlDocument doc)
        {
            XmlElement examNode = doc.CreateElement("Exam");
            examNode.SetAttribute("Series", Series.ToString());
            if (GradeThreshold != null) examNode.SetAttribute("GradeThreshold", GradeThreshold.Url);
            if (ExaminersReport != null) examNode.SetAttribute("ExaminersReport", ExaminersReport.Url);

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

            return examNode;
        }
    }
}
