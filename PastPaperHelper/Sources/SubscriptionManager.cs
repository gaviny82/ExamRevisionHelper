using PastPaperHelper.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;

namespace PastPaperHelper.Sources
{
    public static class SubscriptionManager
    {
        private static readonly XmlDocument subjectList = new XmlDocument();
        private static readonly XmlDocument subscription = new XmlDocument();

        public static SubjectSource[] AllSubjects { get; set; }
        public static Dictionary<SubjectSource, PaperRepository> Subscription { get; set; } = new Dictionary<SubjectSource, PaperRepository>();
        public static PaperSource CurrentPaperSource { get; set; }

        /// <summary>
        /// This method checks update for subject list and subscribed subjects.
        /// Local XML documents are also loaded in this session.
        /// </summary>
        /// <param name="UpdateSubjectList">Output if subject list needs update.</param>
        /// <param name="UpdateSubscription">Output if subscribed subjects needs update.</param>
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

        public static void UpdateAndInit(bool UpdateSubjectList, bool UpdateSubscription)
        {
            if (UpdateSubjectList)
            {
                //Download from web servers
                SubjectSource[] subjects = CurrentPaperSource.GetSubjects();
                PaperSource.SaveSubjectList(subjects, subjectList);
                subjectList.Save(Environment.CurrentDirectory + "\\data\\subjects.xml");
                AllSubjects = subjects;
            }
            else
            {
                //Load from local files
                List<SubjectSource> list = new List<SubjectSource>();
                foreach (XmlNode node in subjectList.SelectNodes("//Subject"))
                {
                    list.Add(new SubjectSource
                    {
                        SubjectInfo = new Subject
                        {
                            Curriculum = node.ParentNode.Name == "IGCSE" ? Curriculums.IGCSE : Curriculums.ALevel,
                            Name = node.Attributes["Name"].Value,
                            SyllabusCode = node.Attributes["SyllabusCode"].Value,
                        },
                        Url = node.Attributes["Url"].Value,
                    }) ;
                }
                AllSubjects = list.ToArray();
            }
            if (UpdateSubscription)
            {
                //Download from web servers
                StringCollection subscriptionStr = Properties.Settings.Default.SubjectsSubcripted;
                Subscription.Clear();
                foreach (string item in subscriptionStr)
                {
                    SubjectSource subject = FindSubject(item, AllSubjects);
                    if (subject != null)
                    {
                        PaperRepository papers = CurrentPaperSource.GetPapers(subject);
                        Subscription.Add(subject, papers);
                    }
                }
                PaperSource.SaveSubscription(Subscription, subscription);
                subscription.Save(Environment.CurrentDirectory + "\\data\\subscription.xml");
            }
            else
            {
                //Load from local files
                //XmlNodeList subjects = subscription.SelectNodes("//Subject");
                //foreach (XmlNode node in subjects)
                //{
                //    List<PaperItem> list = new List<PaperItem>();
                //    SubjectSource subject = FindSubject(node.Attributes["SyllabusCode"].Value, AllSubjects);

                //    if (!Properties.Settings.Default.SubjectsSubcripted.Contains(subject.SubjectInfo.SyllabusCode)) continue;
                //    foreach (XmlNode item in node.ChildNodes)
                //    {
                //        list.Add(new PaperItem
                //        {
                //            Subject = subject,
                //            Year = item.Attributes["Year"].Value,
                //            ExamSeries = (ExamSeries)int.Parse(item.Attributes["ExamSeries"].Value),
                //            ComponentCode = char.Parse(item.Attributes["Component"].Value),
                //            VariantCode = char.Parse(item.Attributes["Variant"].Value),
                //            Type = (FileTypes)int.Parse(item.Attributes["Type"].Value),
                //            Url = item.Attributes["Path"].Value,
                //        });
                //    }
                //    Subscription.Add(subject, list.ToArray());
                //}
            }
        }

        public static SubjectSource FindSubject(string SyllabusCode, SubjectSource[] list)
        {
            foreach (SubjectSource item in list)
            {
                if (item.SubjectInfo.SyllabusCode == SyllabusCode)
                {
                    return item;
                }
            }
            return null;
        }

        public static void Subscribe(SubjectSource subject)
        {

        }

        public static void Unsubscribe(SubjectSource subject)
        {

        }
    }
}
