using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PastPaperHelper.Core.Tools
{
    public enum UpdatePolicy { Disable, Always, Daily, Weekly, Montly, Auto }
    public enum InitializationResult { SuccessNoUpdate, SuccessUpdateNeeded, Error }
    public static class PastPaperHelperCore
    {
        public static Subject[] SubjectsLoaded { get; set; }

        public static ObservableCollection<Subject> SubscribedSubjects = new ObservableCollection<Subject>();
        public static PaperSource Source { get; set; }

        public static string UserDataPath;

        /// <summary>
        /// This function should be called when the PastPaperHelper application starts.
        /// Local user data is loaded, including subjects supported by the current data source, user's subscription and papers of the subscribed subjects, when an XML file path is provided.
        /// </summary>
        /// <param name="source">Initialize a data source</param>
        /// <param name="userDataFilePath">Path of a XML file that stores user data</param>
        /// <param name="updatePolicy">Specifies update frequency (used in checking update)</param>
        /// <param name="subscription">Syllabus codes of subjects subscribed by the user</param>
        /// <returns>Returns true when the local data needs update OR error is detected when loading user_data.xml.
        /// Returns null: error </returns>
        public static InitializationResult Initialize(string userDataFilePath, UpdatePolicy updatePolicy, string[] subscription)
        {
            try
            {
                if (!File.Exists(userDataFilePath))
                {
                    Source = new PaperSourceGCEGuide();
                    return InitializationResult.Error;
                }

                UserDataPath = userDataFilePath;
                XmlDocument userData = new XmlDocument();
                userData.Load(userDataFilePath);

                XmlNode dataNode = userData.SelectSingleNode("/Data");
                if (dataNode == null) return InitializationResult.Error;
                switch (dataNode.Attributes["Source"].Value)
                {
                    default:
                        Source = new PaperSourceGCEGuide();
                        return InitializationResult.Error;
                    case "gce_guide":
                        Source = new PaperSourceGCEGuide(userData);
                        break;
                }

                if (subscription == null) return InitializationResult.SuccessNoUpdate;
                //TODO: Check if the paper source supports all subscribed subjects

                DateTime lastUpdate = Source.LastUpdated;
                double days = (DateTime.Now - lastUpdate).Days;
                switch (updatePolicy)
                {
                    case UpdatePolicy.Disable:
                        return InitializationResult.SuccessNoUpdate;
                    case UpdatePolicy.Always:
                        return InitializationResult.SuccessUpdateNeeded;
                    case UpdatePolicy.Daily:
                        return days < 1 ? InitializationResult.SuccessNoUpdate : InitializationResult.SuccessUpdateNeeded;
                    case UpdatePolicy.Weekly:
                        return days < 7 ? InitializationResult.SuccessNoUpdate : InitializationResult.SuccessUpdateNeeded;
                    case UpdatePolicy.Montly:
                        return days < 30 ? InitializationResult.SuccessNoUpdate : InitializationResult.SuccessUpdateNeeded;
                    case UpdatePolicy.Auto:
                        //TODO: auto update strategy
                        break;
                }

                return InitializationResult.SuccessNoUpdate;
            }
            catch (Exception e)
            {
                return InitializationResult.Error;
            }
        }


        public static void Subscribe(Subject subject)
        {

        }

        public static void Unsubscribe(Subject subject)
        {
           
        }

        public static bool TryFindSubject(string syllabusCode, out Subject result)
        {
            foreach (Subject item in SubjectsLoaded)
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

        public static void UpdateSubjectLoaded()
        {
            SubjectsLoaded = Source.SubjectUrlMap.Keys.ToArray();
        }

    }
}
