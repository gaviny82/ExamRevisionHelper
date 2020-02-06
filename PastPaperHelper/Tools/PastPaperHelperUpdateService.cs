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


        public static async void UpdateAll()
        {
            UpdateInitiatedEvent?.Invoke();

            //Download subject list from web server
            try
            {
                await PastPaperHelperCore.Source.UpdateSubjectUrlMapAsync();
                PastPaperHelperCore.UpdateSubjectLoaded();
            }
            catch (Exception)
            {
                UpdateErrorEvent?.Invoke($"Failed to download subject list from {PastPaperHelperCore.Source.Name}, please check your Internet connection.");
                return;
            }
            UpdateTaskCompleteEvent?.Invoke($"Subject list updated from {PastPaperHelperCore.Source.Name}.");

            //Download papers from web server
            List<Subject> failed = new List<Subject>();
            foreach (Subject subj in PastPaperHelperCore.SubscribedSubjects)
            {
                    try
                    {
                        await PastPaperHelperCore.Source.AddOrUpdateSubject(subj);
                    }
                    catch (Exception)
                    {
                        failed.Add(subj);
                        UpdateErrorEvent?.Invoke($"Failed to update {subj.Name} from {PastPaperHelperCore.Source.Name}.");
                        continue;
                    }
                    UpdateTaskCompleteEvent?.Invoke($"{subj.Name} updated from {PastPaperHelperCore.Source.Name}.");
            }

            try
            {
                //Update finished
                XmlDocument dataDocument = PastPaperHelperCore.Source.SaveDataToXml(PastPaperHelperCore.Source.Subscription);
                dataDocument.Save(PastPaperHelperCore.UserDataPath);

                if (failed.Count == 0) UpdateFinalizedEvent?.Invoke();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to save data to {PastPaperHelperCore.UserDataPath}", e);
            }
        }

        public static async Task UpdateSubjectList()
        {
            UpdateInitiatedEvent?.Invoke();
            try
            {
                await PastPaperHelperCore.Source.UpdateSubjectUrlMapAsync();
                PastPaperHelperCore.UpdateSubjectLoaded();
            }
            catch (Exception)
            {
                UpdateErrorEvent?.Invoke($"Failed to fetch data from {PastPaperHelperCore.Source.Name}, please check your Internet connection.");
                return;
            }
            UpdateFinalizedEvent?.Invoke();
        }

        //TODO: Hot reload on update
        //public static async Task UpdateSubject(Subject subj)
        //{
        //    var downloadThread = Task.Run(() =>
        //    {
        //        return PastPaperHelperCore.Source.GetPapers(subj);
        //    });
        //    //Save to XML
        //    PastPaperHelperCore.Source.Subscription.Add(subj, await downloadThread);
        //}

    }
}
