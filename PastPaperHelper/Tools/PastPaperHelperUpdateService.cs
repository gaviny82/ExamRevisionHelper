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


        public static async void UpdateAll(StringCollection subscription)
        {
            await Task.Run(async() =>
            {
                UpdateInitiatedEvent?.Invoke();

                Dictionary<Subject, string> subjects = null;
                //Download from web servers
                try
                {
                    await PastPaperHelperCore.Source.UpdateSubjectUrlMapAsync();
                    subjects = PastPaperHelperCore.Source.SubjectUrlMap;
                    //PastPaperHelperCore.SubjectsLoaded = subjects.Keys.ToArray();
                }
                catch (Exception)
                {
                    UpdateErrorEvent?.Invoke($"Failed to fetch data from {PastPaperHelperCore.Source.Name}, please check your Internet connection.");
                    return;
                }
                UpdateTaskCompleteEvent?.Invoke($"Subject list updated from {PastPaperHelperCore.Source.Name}.");
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
                            PaperRepository papers = await PastPaperHelperCore.Source.GetPapers(subject);
                            repos.Add(subject, papers);
                        }
                        catch (Exception)
                        {
                            failed.Add(subject);
                            continue;
                        }
                        UpdateTaskCompleteEvent?.Invoke($"{subject.Name} updated from {PastPaperHelperCore.Source.Name}.");
                    }
                }

                //Update finished

                XmlDocument dataDocument = PastPaperHelperCore.Source.SaveDataToXml(repos);
                dataDocument.Save(PastPaperHelperCore.UserDataPath);

                //error message
                if (failed.Count != 0)
                {
                    string failure = "";
                    foreach (var item in failed)
                    {
                        failure += item.Name + ",";
                    }
                    UpdateErrorEvent?.Invoke($"Failed to update {failed.Count} subject{(failed.Count > 1 ? "s" : "")} ({failure.Substring(0,failure.Length-1)}) from {PastPaperHelperCore.Source.Name}, please check your Internet connection.");
                }
                else
                {
                    UpdateFinalizedEvent?.Invoke();
                }
            });
        }

        public static async Task UpdateSubjectList()
        {
            await Task.Run(async () =>
            {
                UpdateInitiatedEvent?.Invoke();

                try
                {
                    await PastPaperHelperCore.Source.UpdateSubjectUrlMapAsync();
                    //PastPaperHelperCore.SubjectsLoaded = PastPaperHelperCore.Source.SubjectUrlMap.Keys.ToArray();
                }
                catch (Exception)
                {
                    UpdateErrorEvent?.Invoke($"Failed to fetch data from {PastPaperHelperCore.Source.Name}, please check your Internet connection.");
                    return;
                }
                UpdateFinalizedEvent?.Invoke();
            });
        }

        public static async Task UpdateSubject(Subject subj)
        {
            var downloadThread = Task.Run(() =>
            {
                return PastPaperHelperCore.Source.GetPapers(subj);
            });
            //Save to XML
            PastPaperHelperCore.Source.Subscription.Add(subj, await downloadThread);
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
