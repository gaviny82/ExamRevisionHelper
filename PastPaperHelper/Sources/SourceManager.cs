using PastPaperHelper.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PastPaperHelper.Sources
{
    public static class SourceManager
    {
        public static SubjectSource[] AllSubjects { get; set; }
        public static Dictionary<SubjectSource, PaperItem[]> Subscription { get; set; } = new Dictionary<SubjectSource, PaperItem[]>();
        public static PaperSource CurrentPaperSource { get; set; }

        public static void CheckUpdate()
        {
            string path = Environment.CurrentDirectory + "\\data";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            if(!File.Exists(path + "\\subjects.xml"))
            {
                //Download subject list when subjects.xml does not exist
                UpdateSubjectList();
                UpdateSubcription();
                return;
            }
            //Loading subject list
            XmlDocument subjectList = new XmlDocument();
            subjectList.Load(path + "\\subjects.xml");
            XmlNode data = subjectList.ChildNodes[1];
            DateTime.TryParse(data.Attributes["Time"].Value, out DateTime lastUpdate);

            if((DateTime.Now - lastUpdate).TotalDays > Properties.Settings.Default.UpdateFrequency)
            {
                //Update if data is expired
                UpdateSubjectList();
            }
            else
            {
                //Load subjects from local files if updated recently
                List<SubjectSource> list = new List<SubjectSource>();
                foreach (XmlNode node in subjectList.SelectNodes("//Subject"))
                {
                    list.Add(new SubjectSource
                    {
                        Curriculum = node.ParentNode.Name == "IGCSE" ? Curriculums.IGCSE : Curriculums.ALevel,
                        Name = node.Attributes["Name"].Value,
                        SyllabusCode = node.Attributes["SyllabusCode"].Value,
                        Url = node.Attributes["Path"].Value,
                    });
                }
                AllSubjects = list.ToArray();
            }

            if(!File.Exists(path + "\\subscription.xml"))
            {
                //TODO: subscription data not found
            }
            XmlDocument subscription = new XmlDocument();
            subscription.Load(path + "\\subscription.xml");
            XmlNode data2 = subscription.ChildNodes[1];
            DateTime.TryParse(data2.Attributes["Time"].Value, out DateTime subscriptUpdate);

            XmlNodeList subjects = subscription.SelectNodes("//Subject");

            bool flag = false;
            foreach (string str in Properties.Settings.Default.SubjectsSubcripted)
            {
                bool isContained = false;
                foreach (XmlNode item in subjects)
                {
                    if (item.Attributes["SyllabusCode"].Value == str)
                    {
                        isContained = true;
                        break;
                    }
                }
                if (!isContained)
                {
                    flag = true;
                    break;
                }
            }
            if ((DateTime.Now - subscriptUpdate).TotalDays > Properties.Settings.Default.UpdateFrequency || flag)
            {
                //Update if data is expired
                UpdateSubcription();
            }
            else
            {
                //Load subscription from local files if updated recently
                foreach (XmlNode node in subjects)
                {
                    List<PaperItem> list = new List<PaperItem>();
                    SubjectSource subject = FindSubject(node.Attributes["SyllabusCode"].Value, AllSubjects);

                    if (!Properties.Settings.Default.SubjectsSubcripted.Contains(subject.SyllabusCode)) continue;
                    foreach (XmlNode item in node.ChildNodes)
                    {
                        list.Add(new PaperItem
                        {
                            Subject = subject,
                            Year = item.Attributes["Year"].Value,
                            ExamSeries = (ExamSeries)int.Parse(item.Attributes["ExamSeries"].Value),
                            ComponentCode = char.Parse(item.Attributes["Component"].Value),
                            VariantCode = char.Parse(item.Attributes["Variant"].Value),
                            Type = (FileTypes)int.Parse(item.Attributes["Type"].Value),
                            Url = item.Attributes["Path"].Value,
                        });
                    }
                    Subscription.Add(subject, list.ToArray());
                }
            }
        }

        public static void UpdateSubjectList()
        {
            SubjectSource[] subjects = CurrentPaperSource.GetSubjects();
            PaperSource.SaveSubjectList(subjects, Environment.CurrentDirectory + "\\data\\subjects.xml", CurrentPaperSource.Name);
            AllSubjects = subjects;
        }

        public static void UpdateSubcription()
        {
            StringCollection subscription = Properties.Settings.Default.SubjectsSubcripted;
            Subscription.Clear();
            foreach (string item in subscription)
            {
                SubjectSource subject = FindSubject(item, AllSubjects);
                if (subject != null)
                {
                    PaperItem[] papers = CurrentPaperSource.GetPapers(subject);
                    Subscription.Add(subject, papers);
                }
            }
            PaperSource.SaveSubscription(Subscription, Environment.CurrentDirectory + "\\data\\subscription.xml", CurrentPaperSource.Name);
        }

        public static SubjectSource FindSubject(string SyllabusCode, SubjectSource[] source)
        {
            foreach (SubjectSource item in source)
            {
                if (item.SyllabusCode == SyllabusCode)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
