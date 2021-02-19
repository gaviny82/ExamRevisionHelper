using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ExamRevisionHelper.Core.Models;
using ExamRevisionHelper.Core.Sources;

namespace ExamRevisionHelper.Core
{
    public enum UpdateFrequency { Disable, Always, Daily, Weekly, Montly, Auto }
    public sealed class PastPaperHelperCore
    {
        #region Experiment
        public XmlDocument UserData { get; set; }//TODO: invoke UserDataChanged event; the listener should write the doc to file.
        public DirectoryInfo LocalFileStorage { get; set; }//TODO: setter should be private
        public PaperSource CurrentSource { get; set; }//TODO: setter should be private
        public Subject[] SubjectsAvailable { get => CurrentSource.SubjectUrlMap.Keys.ToArray(); }
        public Subject[] SubjectsSubscribed { get => SubscriptionRepo.Keys.ToArray(); }
        public Dictionary<Subject, PaperRepository> SubscriptionRepo { get; init; }

        /// <summary>
        /// Initialize source management system and load user data.
        /// </summary>
        /// <param name="userData">XML document which caches past papers information</param>
        /// <param name="localFileStorage">Directory containing local papers</param>
        /// <param name="updatePolicy">Config the update frequency of auto updater</param>
        /// <param name="subscription">A list of strings for syllabus codes of subscribed subjects</param>
        /// <exception cref="SubjectUnsupportedException">when a subject from <paramref name="subscription"/> cannot be found in the repo.</exception>
        /// <exception cref="NotImplementedException">when a <code>PaperSource</code> is not implemented</exception>
        public PastPaperHelperCore([NotNull] XmlDocument userData, [NotNull] DirectoryInfo localFileStorage, [NotNull] UpdateFrequency updatePolicy, [NotNull] IEnumerable<string> subscription)
        {
            //TODO: Make updater a non-static class
            PastPaperHelperUpdateService.Instance = this;

            UserData = userData;
            LocalFileStorage = localFileStorage;

            //Load the paper source from the config file (param: userData)
            string sourceIdentifier = "";
            try
            {
                XmlNode dataNode = userData.SelectSingleNode("/Data");
                sourceIdentifier = dataNode.Attributes["Source"].Value;

                CurrentSource = sourceIdentifier switch
                {
                    PaperSourceGCEGuide.Identifier => new PaperSourceGCEGuide(),
                   "papacambridge" => new PaperSourcePapaCambridge(),
                    "cie_notes" => new PaperSourceCIENotes(),

                    _ => null
                };
                SubscriptionRepo = CurrentSource.LoadUserData(userData);
            }
            catch (Exception e)
            {
                if (CurrentSource is null) throw new NotImplementedException($"This paper source ({sourceIdentifier}) is currently not implemented.");
                throw new Exception($"Failed to load cached data. Try reloading with an appropriate source.", e);
            }
            
            //Checks if syllabus codes provided by parameter matches the repo loaded.
            List<Subject> subjectsInRepo = SubscriptionRepo.Keys.ToList();
            List<string> subscriptionList = new();
            foreach (var item in subscription)
            {
                if (subscriptionList.Contains(item)) continue;
                subscriptionList.Add(item);
            }
            for (int itor = 0; itor < subscriptionList.Count; )
            {
                var item = subscriptionList[0];
                if (PastPaperHelperCore.TryFindSubject(item, out Subject subj, SubjectsAvailable))
                {
                    subjectsInRepo.Remove(subj);
                    subscriptionList.RemoveAt(0);
                }
                else itor++;
            }

            //subjectsInRepo now contains obsolete records of subjects
            //subscriptionList now contains unsupported subjects

            if (subscriptionList.Count != 0)//throws if some subjects are not found in the list (this.SubjectsAvailable)
            {
                var errorMsg = $"The following subjects (in syllabus code) are not supported: {string.Join(',', subscriptionList)}";
                throw new SubjectUnsupportedException(errorMsg) { UnsupportedSubjects = subscriptionList.ToArray() };
            }
        }

        #endregion

        public static Dictionary<string, string> LocalFiles;

        /// <summary>
        /// This function should be called when the PastPaperHelper application starts.
        /// Local user data is loaded, including subjects supported by the current data source, user's subscription and papers of the subscribed subjects, when an XML file path is provided.
        /// </summary>
        /// <param name="userDataFilePath">Path of a XML file that stores user data</param>
        /// <param name="sourceName">Initialize a data source</param>
        /// <param name="updatePolicy">[Not Implemented] Specifies update frequency (used in checking update)</param>
        /// <param name="subscription">Syllabus codes of subjects subscribed by the user</param>
        /// <returns>Returns true when the local data needs update OR error is detected when loading user_data.xml.
        /// Returns null: error </returns>
        //public static InitializationResult Initialize(string userDataFilePath, string localFilesPath, string sourceName, UpdateFrequency updatePolicy, string[] subscription)
        //{
        //    try
        //    {
        //        LocalFilesPath = localFilesPath;
        //        if (!Directory.Exists(localFilesPath))
        //        {
        //            var a = Directory.CreateDirectory(localFilesPath);
        //        }

        //        UserDataPath = userDataFilePath;
        //        XmlDocument userData = new XmlDocument();
        //        userData.Load(userDataFilePath);

        //        XmlNode dataNode = userData.SelectSingleNode("/Data");
        //        Source = dataNode.Attributes["Source"].Value switch
        //        {
        //            "gce_guide" => new PaperSourceGCEGuide(userData),
        //            "papacambridge" => new PaperSourcePapaCambridge(userData),
        //            "cie_notes" => new PaperSourceCIENotes(userData),

        //            _ => throw new NotImplementedException()
        //        };
        //        SubjectsLoaded = Source.SubjectUrlMap.Keys.ToArray();

        //        if (subscription == null) return InitializationResult.SuccessNoUpdate;
        //        LoadSubscribedSubjects(subscription);
        //        foreach (var item in SubscribedSubjects)
        //        {
        //            if (!Source.Subscription.ContainsKey(item))
        //                return InitializationResult.Error;
        //        }
        //        //TODO: prompt if repo of any subscribed subject is not found
        //        //Note: if not supported, throw exception and try reloading. If error still occurred in the reload process, remove this failed subject automatically and notify the user.

        //        return InitializationResult.SuccessNoUpdate;
        //        //TODO: diff to local profile when updated.
        //        DateTime lastUpdate = Source.LastUpdated;
        //        double days = (DateTime.Now - lastUpdate).Days;
        //        switch (updatePolicy)
        //        {
        //            case UpdateFrequency.Disable:
        //                return InitializationResult.SuccessNoUpdate;
        //            case UpdateFrequency.Always:
        //                return InitializationResult.SuccessUpdateNeeded;
        //            case UpdateFrequency.Daily:
        //                return days < 1 ? InitializationResult.SuccessNoUpdate : InitializationResult.SuccessUpdateNeeded;
        //            case UpdateFrequency.Weekly:
        //                return days < 7 ? InitializationResult.SuccessNoUpdate : InitializationResult.SuccessUpdateNeeded;
        //            case UpdateFrequency.Montly:
        //                return days < 30 ? InitializationResult.SuccessNoUpdate : InitializationResult.SuccessUpdateNeeded;
        //            case UpdateFrequency.Auto:
        //                //TODO: auto update strategy
        //                break;
        //        }

        //    }
        //    catch (Exception)
        //    {
        //        Source = sourceName switch
        //        {
        //            "gce_guide" => new PaperSourceGCEGuide(),
        //            "papacambridge" => new PaperSourcePapaCambridge(),
        //            "cie_notes" => new PaperSourceCIENotes(),
        //            _ => new PaperSourceGCEGuide(),
        //        };
        //        UserDataPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\PastPaperHelper\\PastPaperHelper\\{sourceName}.xml";
        //        return InitializationResult.Error;
        //    }
        //}

        public static async Task SaveDataAsync()
        {
            await Task.Run(() =>
            {
                //TODO: XmlDocument doc = Source.SaveDataToXml();
                //doc.Save(UserDataPath);
            });
        }

        //public static void LoadSubscribedSubjects(ICollection<string> subscription)
        //{
        //    List<string> failed = new List<string>();
        //    SubscribedSubjects.Clear();
        //    foreach (string item in subscription)
        //    {
        //        if (!TryFindSubject(item, out Subject subj) || !SubjectsLoaded.Contains(subj))
        //        {
        //            failed.Add(item);
        //        }
        //        else SubscribedSubjects.Add(subj);
        //    }
        //    if (failed.Count != 0)
        //    {
        //        string code = "";
        //        failed.ForEach((str) => { code += str + ","; });
        //        throw new SubjectUnsupportedException($"{code.Substring(0, code.Length - 1)} not supported") { UnsupportedSubjects = failed.ToArray() };
        //    }
        //}

        public static bool TryFindSubject(string syllabusCode, out Subject result, [NotNull] ICollection<Subject> range)
        {
            foreach (Subject item in range)
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
