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
    public enum UpdateFrequency { Disable, Always, Daily, Weekly, Montly, Auto }
    public enum InitializationResult { SuccessNoUpdate, SuccessUpdateNeeded, Error }
    public static class PastPaperHelperCore
    {
        public static Subject[] SubjectsLoaded { get; set; }

        public static ObservableCollection<Subject> SubscribedSubjects { get; private set; } = new ObservableCollection<Subject>();
        public static PaperSource Source { get; set; }
        public static string UserDataPath { get; set; }

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
        public static InitializationResult Initialize(string userDataFilePath, string sourceName, UpdateFrequency updatePolicy, string[] subscription)
        {
            try
            {
                UserDataPath = userDataFilePath;
                XmlDocument userData = new XmlDocument();
                userData.Load(userDataFilePath);

                XmlNode dataNode = userData.SelectSingleNode("/Data");
                switch (dataNode.Attributes["Source"].Value)
                {
                    case "gce_guide":
                        Source = new PaperSourceGCEGuide(userData);
                        break;
                    case "papacambridge":
                        Source = new PaperSourcePapaCambridge(userData);
                        break;
                    case "cie_notes":
                        Source = new PaperSourceCIENotes(userData);
                        break;
                }
                SubjectsLoaded = Source.SubjectUrlMap.Keys.ToArray();

                if (subscription == null) return InitializationResult.SuccessNoUpdate;
                ReloadSubscribedSubjects(subscription);

                DateTime lastUpdate = Source.LastUpdated;
                double days = (DateTime.Now - lastUpdate).Days;
                switch (updatePolicy)
                {
                    case UpdateFrequency.Disable:
                        return InitializationResult.SuccessNoUpdate;
                    case UpdateFrequency.Always:
                        return InitializationResult.SuccessUpdateNeeded;
                    case UpdateFrequency.Daily:
                        return days < 1 ? InitializationResult.SuccessNoUpdate : InitializationResult.SuccessUpdateNeeded;
                    case UpdateFrequency.Weekly:
                        return days < 7 ? InitializationResult.SuccessNoUpdate : InitializationResult.SuccessUpdateNeeded;
                    case UpdateFrequency.Montly:
                        return days < 30 ? InitializationResult.SuccessNoUpdate : InitializationResult.SuccessUpdateNeeded;
                    case UpdateFrequency.Auto:
                        //TODO: auto update strategy
                        break;
                }

                return InitializationResult.SuccessNoUpdate;
            }
            catch (Exception)
            {
                switch (sourceName)
                {
                    default:
                        Source = new PaperSourceGCEGuide();
                        break;
                    case "gce_guide":
                        Source = new PaperSourceGCEGuide();
                        break;
                    case "papacambridge":
                        Source = new PaperSourcePapaCambridge();
                        break;
                    case "cie_notes":
                        Source = new PaperSourceCIENotes();
                        break;
                }
                UserDataPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\PastPaperHelper\\PastPaperHelper\\{sourceName}.xml";
                return InitializationResult.Error;
            }
        }

        public static void ReloadSubscribedSubjects(string[] subscription)
        {
            foreach (string item in subscription)
            {
                if (!TryFindSubject(item, out Subject subj) || !SubjectsLoaded.Contains(subj))
                {
                    //TODO: Unsupported subject handling, remove from subscription automatically
                    throw new Exception($"{item} not supported");
                }
                SubscribedSubjects.Add(subj);
            }
        }

        public static bool TryFindSubject(string syllabusCode, out Subject result, ICollection<Subject> range = null)
        {
            foreach (Subject item in range ?? SubjectsLoaded)
            {
                if (item.SyllabusCode == syllabusCode)
                {
                    result = item;
                    return true;
                }
            }
            result = null;
            return false;
        }

    }
}
