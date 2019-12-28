using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PastPaperHelper.Core.Tools
{
    public enum UpdateStatus { NoUpdate, Updated, UpdateFailed }
    public static class PastPaperHelperCore
    {
        public static Subject[] SubjectsLoaded { get; set; }
        public static Dictionary<Subject, PaperRepository> Subscription { get; set; }
        private static PaperSource CurrentSource { get; set; }

        private static XmlDocument userData;
        private static string userDataPath;

        /// <summary>
        /// This function should be called when the PastPaperHelper application starts.
        /// </summary>
        /// <param name="source">Initialize a data source</param>
        /// <param name="userDataPath">Path of a XML file that stores user data</param>
        /// <param name="updatePolicy">Specifies update frequency (used in checking update)</param>
        /// <param name="subscription">Syllabus codes of subjects subscribed by the user</param>
        /// <returns>Returns true when the local data needs update OR error is detected when loading user_data.xml.
        /// Returns null: error </returns>
        public static bool? Initialize(PaperSource source, string userDataPath, UpdatePolicy updatePolicy, string[] subscription)
        {
            CurrentSource = source;
            PastPaperHelperCore.userDataPath = userDataPath;
            userData = new XmlDocument();

            if (File.Exists(userDataPath))
            {
                userData.Load(userDataPath);
                XmlNode updateInfo = userData.SelectSingleNode("/UpdateInfo");
                if (updateInfo == null || updateInfo.Attributes["LastUpdate"] == null) return null;
                else
                {
                    //Load subject list
                    DateTime.TryParse(updateInfo.Attributes["LastUpdate"].Value, out DateTime lastUpdate);
                    if ((DateTime.Now - lastUpdate).TotalDays > 100) return true;//TODO: set update frequency

                    XmlNodeList nodes = userData.SelectNodes("/SubjectList/Subject");
                    if (nodes == null) return true;

                    SubjectsLoaded = new Subject[nodes.Count];
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        XmlNode node = nodes[i];
                        Subject subj = new Subject
                        {
                            Curriculum = node.ParentNode.Name == "IGCSE" ? Curriculums.IGCSE : Curriculums.ALevel,
                            Name = node.Attributes["Name"].Value,
                            SyllabusCode = node.Attributes["SyllabusCode"].Value,
                        };
                        //SubjectUrlMap.Add(subj, node.Attributes["Url"].Value); TODO: initialize the paper source using the cached data
                        SubjectsLoaded[i] = subj;
                    }

                    //Load cached repositories of subscribed subjects
                    foreach (XmlNode subjectNode in userData.SelectNodes("/Subscription/Repository"))
                    {
                        PaperRepository repo = new PaperRepository(subjectNode.Attributes["SyllabusCode"].Value);
                        if (!subscription.Contains(repo.Subject.SyllabusCode)) continue;

                        foreach (XmlNode yearNode in subjectNode.ChildNodes)
                        {
                            ExamYear year = new ExamYear { Year = yearNode.Attributes["Year"].Value };
                            if (yearNode.Attributes["Syllabus"] != null) year.Syllabus = new Syllabus { Year = year.Year, Url = yearNode.Attributes["Syllabus"].Value };

                            foreach (XmlNode examNode in yearNode.ChildNodes)
                            {
                                Exam exam = new Exam(examNode, repo.Subject);
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
                        Subscription.Add(repo.Subject, repo);
                    }
                }
                return false;//TODO: Add try-catch block to prevent crashing when file format is upgraded
            }
            else return null;
        }

        public static async Task Update(Subject[] subscription)
        {
            //Dictionary<Subject,string> subjUrlMap = CurrentSource.GetSubjectUrlMap();
            //SubjectsLoaded = subjUrlMap.Keys.ToArray();
            //foreach (Subject subj in subscription)
            //{
            //    Subscription.Add(subj, CurrentSource.GetPapers(subj));
            //}
        }

        public static void Subscribe(Subject subject)
        {

        }

        public static void Unsubscribe(Subject subject)
        {
           
        }

    }
}
