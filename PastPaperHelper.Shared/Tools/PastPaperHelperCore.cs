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
    public enum InitializationResult { SuccessNoUpdate, SuccessUpdateNeeded, Error }
    public static class PastPaperHelperCore
    {
        public static DateTime LastUpdated { get; private set; }
        public static Subject[] SubjectsLoaded { get; set; }

        public static Dictionary<Subject, string> SubjectUrlMap { get; set; }
        public static Dictionary<Subject, PaperRepository> Subscription { get; set; }
        public static PaperSource CurrentSource { get; set; }

        private static XmlDocument userData;
        public static string UserDataPath;

        /// <summary>
        /// This function should be called when the PastPaperHelper application starts.
        /// Local user data is loaded, including subjects supported by the current data source, user's subscription and papers of the subscribed subjects, when an XML file path is provided.
        /// </summary>
        /// <param name="source">Initialize a data source</param>
        /// <param name="userDataPath">Path of a XML file that stores user data</param>
        /// <param name="updatePolicy">Specifies update frequency (used in checking update)</param>
        /// <param name="subscription">Syllabus codes of subjects subscribed by the user</param>
        /// <returns>Returns true when the local data needs update OR error is detected when loading user_data.xml.
        /// Returns null: error </returns>
        public static InitializationResult Initialize(PaperSource source, string userDataPath, UpdatePolicy updatePolicy, string[] subscription)
        {
            CurrentSource = source;
            UserDataPath = userDataPath;
            userData = new XmlDocument();

            if (File.Exists(userDataPath))
            {
                userData.Load(userDataPath);
                XmlNode updateInfo = userData.SelectSingleNode("/Data");
                if (updateInfo == null || updateInfo.Attributes["LastUpdate"] == null) return InitializationResult.Error;
                else
                {
                    //Load subject list
                    DateTime.TryParse(updateInfo.Attributes["LastUpdate"].Value, out DateTime lastUpdate);
                    LastUpdated = lastUpdate;
                    if ((DateTime.Now - lastUpdate).TotalDays > 100) return InitializationResult.SuccessUpdateNeeded;//TODO: set update frequency

                    XmlNodeList nodes = userData.SelectNodes("/Data/SubjectList/Subject");
                    if (nodes == null) return InitializationResult.Error;

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

                    //TODO: Check if the paper source supports all subscribed subjects
                    if (subscription == null) return InitializationResult.SuccessNoUpdate;
                    foreach (XmlNode subjectNode in userData.SelectNodes("/Data/Subscription/Repository"))
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
                return InitializationResult.SuccessNoUpdate;
                //TODO: Add try-catch block to prevent crashing when file format is upgraded
            }
            else return InitializationResult.Error;
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
