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
    public sealed class ExamRevisionHelperCore
    {
        public XmlDocument UserData { get; set; }//TODO: invoke UserDataChanged event; the listener should write the doc to file.
        public DirectoryInfo LocalFileStorage { get; set; }//TODO: setter should be private
        public PaperSource CurrentSource { get; set; }//TODO: setter should be private
        public Subject[] SubjectsAvailable { get => CurrentSource.SubjectUrlMap.Keys.ToArray(); }
        public Subject[] SubjectsSubscribed { get => SubscriptionRepo.Keys.ToArray(); }
        public Dictionary<Subject, PaperRepository> SubscriptionRepo { get; init; }
        public ExamRevisionHelperUpdater Updater { get; init; }

        /// <summary>
        /// Initialize source management system and load user data.
        /// </summary>
        /// <param name="userData">XML document which caches past papers information</param>
        /// <param name="localFileStorage">Directory containing local papers</param>
        /// <param name="updatePolicy">Config the update frequency of auto updater</param>
        /// <param name="subscription">A list of strings for syllabus codes of subscribed subjects</param>
        /// <exception cref="SubjectUnsupportedException">when a subject from <paramref name="subscription"/> cannot be found in the repo.</exception>
        /// <exception cref="NotImplementedException">when a <code>PaperSource</code> is not implemented</exception>
        public ExamRevisionHelperCore([NotNull] XmlDocument userData, [NotNull] DirectoryInfo localFileStorage, [NotNull] UpdateFrequency updatePolicy, [NotNull] IEnumerable<string> subscription)
        {
            Updater = new ExamRevisionHelperUpdater(this);
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
                if (CurrentSource is null) throw new NotImplementedException($"Failed to load data from the source: {sourceIdentifier}.");
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
                if (ExamRevisionHelperCore.TryFindSubject(item, out Subject subj, SubjectsAvailable))
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

            //TODO: diff to local profile when updated.
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
        }

        public static Dictionary<string, string> LocalFiles;

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
