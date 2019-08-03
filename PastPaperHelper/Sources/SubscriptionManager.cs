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
                        Subscription.Add(subject.SubjectInfo, papers);
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
                    Subject subject = FindSubject(subjectNode.Attributes["SyllabusCode"].Value, AllSubjects).SubjectInfo;
                    PaperRepository repo = new PaperRepository(subject);
                    if (!Properties.Settings.Default.SubjectsSubcripted.Contains(subject.SyllabusCode)) continue;

                    List<PaperItem> syllabuses = new List<PaperItem>();
                    foreach (XmlNode syllabusNode in subjectNode.SelectNodes("//Syllabus"))
                    {
                        //init syllabuses
                        syllabuses.Add(new PaperItem
                        {
                            Type = FileTypes.Syllabus,
                            Exam = new Exam { Subject = subject, Year = syllabusNode.Attributes["Year"].Value },
                            Url = syllabusNode.Attributes["Url"].Value
                        });
                    }
                    repo.Syllabus = syllabuses.ToArray();

                    List<Exam> exams = new List<Exam>();
                    foreach (XmlNode examNode in subjectNode.SelectNodes("//ExamSeries"))
                    {
                        List<PaperItem> papers = new List<PaperItem>();
                        Exam exam = new Exam
                        {
                            ExamSeries = (ExamSeries)int.Parse(examNode.Attributes["Series"].Value),
                            Subject = subject,
                            Year = examNode.Attributes["Year"].Value
                        };

                        foreach (XmlNode paperNode in examNode.SelectNodes("//Paper"))
                        {
                            papers.Add(new PaperItem
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
