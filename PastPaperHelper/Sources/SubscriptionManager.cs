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

        public static Subject[] AllSubjects { get; set; }
        public static Dictionary<Subject, string> SubjectUrlMap { get; private set; } = new Dictionary<Subject, string>();
        public static Dictionary<Subject, PaperRepository> Subscription { get; set; } = new Dictionary<Subject, PaperRepository>();
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
                Dictionary<Subject, string> subjects = CurrentPaperSource.GetSubjectUrlMap();
                PaperSource.SaveSubjectList(subjects, subjectList);
                subjectList.Save(Environment.CurrentDirectory + "\\data\\subjects.xml");

                Subject[] arrSubjects = new Subject[subjects.Count];
                subjects.Keys.CopyTo(arrSubjects, 0);
                AllSubjects = arrSubjects;
                SubjectUrlMap = subjects;
            }
            else
            {
                //Load from local files
                Dictionary<Subject, string> list = new Dictionary<Subject, string>();
                foreach (XmlNode node in subjectList.SelectNodes("//Subject"))
                {
                    list.Add(new Subject
                    {
                        Curriculum = node.ParentNode.Name == "IGCSE" ? Curriculums.IGCSE : Curriculums.ALevel,
                        Name = node.Attributes["Name"].Value,
                        SyllabusCode = node.Attributes["SyllabusCode"].Value,
                    }, node.Attributes["Url"].Value);
                }
                Subject[] arrSubjects = new Subject[list.Count];
                list.Keys.CopyTo(arrSubjects, 0);
                AllSubjects = arrSubjects;
                SubjectUrlMap = list;
            }
            if (UpdateSubscription)
            {
                //Download from web servers
                StringCollection subscriptionStr = Properties.Settings.Default.SubjectsSubcripted;
                Subscription.Clear();
                foreach (string item in subscriptionStr)
                {
                    if (TryFindSubject(item, AllSubjects, out Subject subject))
                    {
                        PaperRepository papers = CurrentPaperSource.GetPapers(subject, SubjectUrlMap[subject]);
                        Subscription.Add(subject, papers);
                    }
                }
                PaperSource.SaveSubscription(Subscription, subscription);
                subscription.Save(Environment.CurrentDirectory + "\\data\\subscription.xml");
            }
            else
            {
                //Load from local files//TODO: new format to store examiners reports and grade thresholds
                foreach (XmlNode subjectNode in subscription.SelectNodes("//Subject"))
                {
                    TryFindSubject(subjectNode.Attributes["SyllabusCode"].Value, AllSubjects, out Subject subject);
                    PaperRepository repo = new PaperRepository(subject);
                    if (!Properties.Settings.Default.SubjectsSubcripted.Contains(subject.SyllabusCode)) continue;

                    List<Syllabus> syllabuses = new List<Syllabus>();
                    foreach (XmlNode syllabusNode in subjectNode.SelectNodes("//Syllabus"))
                    {
                        //init syllabuses
                        syllabuses.Add(new Syllabus
                        {
                            Year = syllabusNode.Attributes["Year"].Value,
                            Url = syllabusNode.Attributes["Url"].Value
                        });
                    }
                    repo.Syllabus = syllabuses.ToArray();

                    List<Exam> exams = new List<Exam>();
                    foreach (XmlNode examNode in subjectNode.SelectNodes("//ExamSeries"))
                    {
                        List<Paper> papers = new List<Paper>();
                        Exam exam = new Exam
                        {
                            Series = (ExamSeries)int.Parse(examNode.Attributes["Series"].Value),
                            Subject = subject,
                            Year = examNode.Attributes["Year"].Value
                        };

                        foreach (XmlNode paperNode in examNode.SelectNodes("//Paper"))
                        {
                            papers.Add(new Paper
                            {
                                Exam = exam,
                                ComponentCode = char.Parse(paperNode.Attributes["Component"].Value),
                                VariantCode = char.Parse(paperNode.Attributes["Variant"].Value),
                                Type = (FileTypes)int.Parse(paperNode.Attributes["Type"].Value),
                                Url = paperNode.Attributes["Url"].Value
                            });
                        }
                        exam.Papers = papers.ToArray();
                        exams.Add(exam);
                    }
                    repo.Exams = exams.ToArray();
                    Subscription.Add(subject, repo);
                }
            }
        }

        public static bool TryFindSubject(string SyllabusCode, Subject[] list, out Subject result)
        {
            foreach (Subject item in list)
            {
                if (item.SyllabusCode == SyllabusCode)
                {
                    result = item;
                    return true;
                }
            }
            result = new Subject();
            return false;
        }

        public static void Subscribe(Subject subject)
        {

        }

        public static void Unsubscribe(Subject subject)
        {

        }
    }
}
