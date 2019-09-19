using PastPaperHelper.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace PastPaperHelper.Sources
{
    public static class SubscriptionManager
    {
        private static readonly XmlDocument subjectList = new XmlDocument();
        private static readonly XmlDocument subscription = new XmlDocument();

        public static Subject[] AllSubjects { get; set; }
        public static Dictionary<Subject, string> SubjectUrlMap { get; private set; } = new Dictionary<Subject, string>();
        public static Dictionary<Subject, PaperRepository> Subscription { get; set; } = new Dictionary<Subject, PaperRepository>();

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
                SubjectUrlMap.Clear();
                Dictionary<Subject, string> subjects = PaperSource.CurrentPaperSource.GetSubjectUrlMap();
                AllSubjects = new Subject[subjects.Count];
                subjects.Keys.CopyTo(AllSubjects, 0);
                SubjectUrlMap = subjects;

                PaperSource.SaveSubjectList(SubjectUrlMap, subjectList);
                subjectList.Save(Environment.CurrentDirectory + "\\data\\subjects.xml");
            }
            else
            {
                //Load from local files
                SubjectUrlMap.Clear();
                XmlNodeList nodes = subjectList.SelectNodes("//Subject");
                AllSubjects = new Subject[nodes.Count];
                int i = 0;
                foreach (XmlNode node in nodes)
                {
                    Subject subj = new Subject
                    {
                        Curriculum = node.ParentNode.Name == "IGCSE" ? Curriculums.IGCSE : Curriculums.ALevel,
                        Name = node.Attributes["Name"].Value,
                        SyllabusCode = node.Attributes["SyllabusCode"].Value,
                    };
                    SubjectUrlMap.Add(subj, node.Attributes["Url"].Value);
                    AllSubjects[i++] = subj;
                }
            }
            if (UpdateSubscription)
            {
                //Download from web servers
                StringCollection subscriptionStr = Properties.Settings.Default.SubjectsSubcripted;
                Subscription.Clear();
                foreach (string item in subscriptionStr)
                {
                    if (TryFindSubject(item, out Subject subject))
                    {
                        PaperRepository papers = PaperSource.CurrentPaperSource.GetPapers(subject, SubjectUrlMap[subject]);
                        Subscription.Add(subject, papers);
                    }
                }
                PaperSource.SaveSubscription(Subscription, subscription);
                subscription.Save(Environment.CurrentDirectory + "\\data\\subscription.xml");
            }
            else
            {
                //Load from local files
                foreach (XmlNode subjectNode in subscription.SelectNodes("//Subject"))
                {
                    TryFindSubject(subjectNode.Attributes["SyllabusCode"].Value, out Subject subject);
                    PaperRepository repo = new PaperRepository(subject);
                    if (!Properties.Settings.Default.SubjectsSubcripted.Contains(subject.SyllabusCode)) continue;

                    foreach (XmlNode yearNode in subjectNode.ChildNodes)
                    {
                        ExamYear year = new ExamYear { Year = yearNode.Attributes["Year"].Value };
                        if (yearNode.Attributes["Syllabus"] != null) year.Syllabus = new Syllabus { Year = year.Year, Url = yearNode.Attributes["Syllabus"].Value };

                        foreach (XmlNode examNode in yearNode.ChildNodes)
                        {
                            Exam exam = new Exam(examNode, subject);
                            switch (exam.Series)
                            {
                                case ExamSeries.Spring:
                                    year.Spring = exam;
                                    break;
                                case ExamSeries.Summer:
                                    year.Summer = exam;
                                    break;
                                case ExamSeries.Winter:
                                    year.Winter = exam;
                                    break;
                                default:
                                    year.Specimen = exam;
                                    break;
                            }
                        }
                        repo.Add(year);
                    }

                    repo.Sort();
                    Subscription.Add(subject, repo);
                }
            }
        }

        public static bool TryFindSubject(string syllabusCode, out Subject result)
        {
            foreach (Subject item in AllSubjects)
            {
                if (item.SyllabusCode == syllabusCode)
                {
                    result = item;
                    return true;
                }
            }
            result = new Subject();
            return false;
        }

        public static async Task<bool> Subscribe(Subject subject)
        {
            if (Properties.Settings.Default.SubjectsSubcripted.Contains(subject.SyllabusCode)) return false;

            Properties.Settings.Default.SubjectsSubcripted.Add(subject.SyllabusCode);
            Properties.Settings.Default.Save();

            if (Subscription.ContainsKey(subject)) return false;

            Task<PaperRepository> t = Task.Run(() =>
            {
                return PaperSource.CurrentPaperSource.GetPapers(subject, SubjectUrlMap[subject]);
            });

            PaperRepository repo = await t;
            Subscription.Add(subject, repo);
            return true;
        }

        public static void Unsubscribe(Subject subject)
        {
            if (!Subscription.ContainsKey(subject)) return;

            Subscription.Remove(subject);
            Properties.Settings.Default.SubjectsSubcripted.Remove(subject.SyllabusCode);
            Properties.Settings.Default.Save();
        }
    }
}
