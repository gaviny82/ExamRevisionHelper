using PastPaperHelper.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;

namespace PastPaperHelper.Sources
{
    public static class SourceManager
    {
        //TODO: initialize xml documents as global variables
        public static SubjectSource[] AllSubjects { get; set; }
        public static Dictionary<SubjectSource, PaperItem[]> Subscription { get; set; } = new Dictionary<SubjectSource, PaperItem[]>();
        public static PaperSource CurrentPaperSource { get; set; }

        public static void CheckUpdate(out bool UpdateSubjectList, out bool UpdateSubscription)
        {
            UpdateSubjectList = false;
            UpdateSubscription = false;
            string path = Environment.CurrentDirectory + "\\data";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            //Check update for subject list
            if (!File.Exists(path + "\\subjects.xml"))
            {
                //Download subject list when subjects.xml does not exist
                UpdateSubjectList = true;
                UpdateSubscription = true;
                return;
            }

            XmlDocument subjectList = new XmlDocument();
            subjectList.Load(path + "\\subjects.xml");
            XmlNode data = subjectList.ChildNodes[1];
            DateTime.TryParse(data.Attributes["Time"].Value, out DateTime lastUpdate);

            if ((DateTime.Now - lastUpdate).TotalDays > Properties.Settings.Default.UpdateFrequency)
            {
                //Update if subject data expired
                UpdateSubjectList = true;
            }

            //Check update for subscribed subjects
            if (!File.Exists(path + "\\subscription.xml"))
            {
                //Update subscription if subscription.xml does not exist
                UpdateSubscription = true;
            }
            else
            {
                XmlDocument subscription = new XmlDocument();
                subscription.Load(path + "\\subscription.xml");
                XmlNode data2 = subscription.ChildNodes[1];
                DateTime.TryParse(data2.Attributes["Time"].Value, out DateTime subscriptUpdate);

                XmlNodeList subjects = subscription.SelectNodes("//Subject");
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
                        //Update subscription if subscribed subject data is not included in local files
                        UpdateSubscription = true;
                        break;
                    }
                }
                if ((DateTime.Now - subscriptUpdate).TotalDays > Properties.Settings.Default.UpdateFrequency)
                {
                    //Update if data expired
                    UpdateSubscription = true;
                }
            }
        }

        public static void UpdateAndLoad(bool UpdateSubjectList, bool UpdateSubscription)
        {
            if (UpdateSubjectList)
            {
                //Download from web servers
                SubjectSource[] subjects = CurrentPaperSource.GetSubjects();
                PaperSource.SaveSubjectList(subjects, Environment.CurrentDirectory + "\\data\\subjects.xml", CurrentPaperSource.Name);
                AllSubjects = subjects;
            }
            else
            {
                //Load from local files
                XmlDocument subjectList = new XmlDocument();
                subjectList.Load(Environment.CurrentDirectory + "\\data\\subjects.xml");

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
            if (UpdateSubscription)
            {
                //Download from web servers
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
            else
            {
                //Load from local files
                XmlDocument subscription = new XmlDocument();
                subscription.Load(Environment.CurrentDirectory + "\\data\\subscription.xml");
                XmlNodeList subjects = subscription.SelectNodes("//Subject");
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

        public static SubjectSource FindSubject(string SyllabusCode, SubjectSource[] list)
        {
            foreach (SubjectSource item in list)
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
