using PastPaperHelper.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Xml;
using System.Collections.Specialized;

namespace PastPaperHelper.Core.Tools
{
    public enum UpdatePolicy { Disable, Always, Daily, Weekly, Montly, Auto }
    public static class PastPaperHelperUpdateService
    {
        public delegate void UpdateInitiatedEventHandler();

        public static event UpdateInitiatedEventHandler UpdateInitiatedEvent;


        public delegate void UpdateTaskCompleteEventHandler(string status);

        public static event UpdateTaskCompleteEventHandler UpdateTaskCompleteEvent;


        public delegate void UpdateErrorEventHandler(string status);

        public static event UpdateErrorEventHandler UpdateErrorEvent;


        public delegate void UpdateFinalizedEventHandler();

        public static event UpdateInitiatedEventHandler UpdateFinalizedEvent;


        public static void UpdateAll(StringCollection subscription)
        {
            Task.Run(() =>
            {
                UpdateInitiatedEvent?.Invoke();

                XmlDocument dataDocument = new XmlDocument();
                Dictionary<Subject, string> subjects = null;
                //Download from web servers
                try
                {
                    subjects = PastPaperHelperCore.CurrentSource.GetSubjectUrlMap();
                    PastPaperHelperCore.SubjectsLoaded = subjects.Keys.ToArray();
                    PastPaperHelperCore.SubjectUrlMap = subjects;
                }
                catch (Exception)
                {
                    UpdateErrorEvent?.Invoke($"Failed to fetch data from {PastPaperHelperCore.CurrentSource.Name}, please check your Internet connection.");
                    return;
                }
                UpdateTaskCompleteEvent?.Invoke($"Subject list updated from {PastPaperHelperCore.CurrentSource.Name}.");
                //TODO: in oobe

                //Download from web servers
                Dictionary<Subject, PaperRepository> repos = new Dictionary<Subject, PaperRepository>();
                List<Subject> failed = new List<Subject>();
                foreach (string item in subscription)
                {
                    if (TryFindSubject(item, out Subject subject))
                    {
                        try
                        {
                            PaperRepository papers = PastPaperHelperCore.CurrentSource.GetPapers(subject, PastPaperHelperCore.SubjectUrlMap[subject]);
                            repos.Add(subject, papers);
                        }
                        catch (Exception)
                        {
                            failed.Add(subject);
                            continue;
                        }
                        UpdateTaskCompleteEvent?.Invoke($"{subject.Name} updated from {PastPaperHelperCore.CurrentSource.Name}.");
                    }
                }

                //Update finished
                PastPaperHelperCore.CurrentSource.Save(subjects, repos, dataDocument);
                dataDocument.Save(PastPaperHelperCore.UserDataPath);

                //error message
                if (failed.Count != 0)
                {
                    string failure = "";
                    foreach (var item in failed)
                    {
                        failure += item.Name + ",";
                    }
                    UpdateErrorEvent?.Invoke($"Failed to update {failed.Count} subject{(failed.Count > 1 ? "s" : "")} ({failure[0..^1]}) from {PastPaperHelperCore.CurrentSource.Name}, please check your Internet connection.");
                }
                else
                {
                    UpdateFinalizedEvent?.Invoke();
                }
            });
        }

        public static async Task UpdateSubject(Subject subj)
        {
            var downloadThread = Task.Run(() =>
            {
                return PastPaperHelperCore.CurrentSource.GetPapers(subj, PastPaperHelperCore.SubjectUrlMap[subj]);
            });
            //Save to XML
            PastPaperHelperCore.Subscription.Add(subj, await downloadThread);
        }

        public static bool TryFindSubject(string syllabusCode, out Subject result)
        {
            foreach (Subject item in PastPaperHelperCore.SubjectsLoaded)
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

    }
}
