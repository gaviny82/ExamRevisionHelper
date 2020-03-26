using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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

        public static List<Subject> SubscribedSubjects { get; private set; } = new List<Subject>();
        public static PaperSource Source { get; set; }
        public static string UserDataPath { get; set; }
        public static string LocalFilesPath { get; set; }

        public static Dictionary<string, string> LocalFiles;

        /// <summary>
        /// This function should be called when the PastPaperHelper application starts.
        /// Local user data is loaded, including subjects supported by the current data source, user's subscription and papers of the subscribed subjects, when an XML file path is provided.
        /// </summary>
        /// <param name="userDataFilePath">Path of a XML file that stores user data</param>
        /// <param name="sourceName">Initialize a data source</param>
        /// <param name="updatePolicy">Specifies update frequency (used in checking update)</param>
        /// <param name="subscription">Syllabus codes of subjects subscribed by the user</param>
        /// <returns>Returns true when the local data needs update OR error is detected when loading user_data.xml.
        /// Returns null: error </returns>
        public static InitializationResult Initialize(string userDataFilePath, string localFilesPath, string sourceName, UpdateFrequency updatePolicy, string[] subscription)
        {
            try
            {
                LocalFilesPath = localFilesPath;
                if (!Directory.Exists(localFilesPath))
                {
                    Directory.CreateDirectory(localFilesPath);
                }

                UserDataPath = userDataFilePath;
                XmlDocument userData = new XmlDocument();
                userData.Load(userDataFilePath);

                XmlNode dataNode = userData.SelectSingleNode("/Data");
                Source = dataNode.Attributes["Source"].Value switch
                {
                    "gce_guide" => new PaperSourceGCEGuide(userData),
                    "papacambridge" => new PaperSourcePapaCambridge(userData),
                    "cie_notes" => new PaperSourceCIENotes(userData),

                    _ => throw new NotImplementedException()
                };
                SubjectsLoaded = Source.SubjectUrlMap.Keys.ToArray();

                if (subscription == null) return InitializationResult.SuccessNoUpdate;
                LoadSubscribedSubjects(subscription);
                foreach (var item in SubscribedSubjects)
                {
                    if (!Source.Subscription.ContainsKey(item))
                        return InitializationResult.Error;
                }
                //TODO: prompt if repo of any subscribed subject is not found
                //Note: if not supported, throw exception and try reloading. If error still occurred in the reload process, remove this failed subject automatically and notify the user.

                return InitializationResult.SuccessNoUpdate;
                //TODO: diff to local profile when updated.
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

            }
            catch (Exception)
            {
                Source = sourceName switch
                {
                    "gce_guide" => new PaperSourceGCEGuide(),
                    "papacambridge" => new PaperSourcePapaCambridge(),
                    "cie_notes" => new PaperSourceCIENotes(),
                    _ => new PaperSourceGCEGuide(),
                };
                UserDataPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\PastPaperHelper\\PastPaperHelper\\{sourceName}.xml";
                return InitializationResult.Error;
            }
        }

        public static async Task SaveDataAsync()
        {
            await Task.Run(() =>
            {
                XmlDocument doc = Source.SaveDataToXml();
                doc.Save(UserDataPath);
            });
        }

        public static void LoadSubscribedSubjects(ICollection<string> subscription)
        {
            List<string> failed = new List<string>();
            SubscribedSubjects.Clear();
            foreach (string item in subscription)
            {
                if (!TryFindSubject(item, out Subject subj) || !SubjectsLoaded.Contains(subj))
                {
                    failed.Add(item);
                }
                else SubscribedSubjects.Add(subj);
            }
            if (failed.Count != 0)
            {
                string code = "";
                failed.ForEach((str) => { code += str + ","; });
                throw new SubjectUnsupportedException($"{code.Substring(0, code.Length - 1)} not supported") { UnsupportedSubjects = failed.ToArray() };
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
