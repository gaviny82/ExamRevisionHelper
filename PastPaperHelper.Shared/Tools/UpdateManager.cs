using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.Core.Tools
{
    public enum UpdatePolicy { Disable, Always, Daily, Weekly, Montly, Auto }
    public static class UpdateManager
    {
        public static async Task<bool> CheckUpdateSubjectListAsync(DateTime lastUpdate, UpdatePolicy policy)
        {
            switch (policy)
            {
                case UpdatePolicy.Disable:
                    return false;
                case UpdatePolicy.Always:
                    return true;
                case UpdatePolicy.Auto:
                    return false;//Not implemented. TODO: Smart update
                case UpdatePolicy.Daily:
                    break;
                case UpdatePolicy.Weekly:
                    break;
                case UpdatePolicy.Montly:
                    break;
                default:
                    break;
            }
            await Task.Run(()=> { });
            return false;
        }

        public static async Task UpdateAll()
        {
            //if (UpdateSubjectList)
            //{
            //    //Download from web servers
            //    try
            //    {
            //        SubjectUrlMap.Clear();
            //        Dictionary<Subject, string> subjects = PaperSource.CurrentPaperSource.GetSubjectUrlMap();
            //        AllSubjects = new Subject[subjects.Count];
            //        subjects.Keys.CopyTo(AllSubjects, 0);
            //        SubjectUrlMap = subjects;

            //        PaperSource.SaveSubjectList(SubjectUrlMap, subjectList);
            //        subjectList.Save(Environment.CurrentDirectory + "\\data\\subjects.xml");
            //    }
            //    catch (Exception)
            //    {
            //        Task.Factory.StartNew(() => MainWindow.MainSnackbar.MessageQueue.Enqueue("Failed to fetch data from " + PaperSource.CurrentPaperSource.Name + ", please check your Internet connection."), new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
            //        UpdateAndInit(false, UpdateSubscription);
            //        return;
            //    }
            //    Task.Factory.StartNew(() => MainWindow.MainSnackbar.MessageQueue.Enqueue("Subject list updated from " + PaperSource.CurrentPaperSource.Name), new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
            //    //FIXME: in oobe
            //}
            //if (UpdateSubscription)
            //{
            //    //Download from web servers
            //    StringCollection subscriptionStr = Properties.Settings.Default.SubjectsSubcripted;
            //    Subscription.Clear();
            //    foreach (string item in subscriptionStr)
            //    {
            //        if (TryFindSubject(item, out Subject subject))
            //        {
            //            try
            //            {
            //                PaperRepository papers = PaperSource.CurrentPaperSource.GetPapers(subject, SubjectUrlMap[subject]);
            //                Subscription.Add(subject, papers);
            //            }
            //            catch (Exception)
            //            {
            //                Task.Factory.StartNew(() => MainWindow.MainSnackbar.MessageQueue.Enqueue("Failed to update" + subject.Name + " from " + PaperSource.CurrentPaperSource.Name + ", please check your Internet connection."), new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
            //                continue;
            //            }
            //            Task.Factory.StartNew(() => MainWindow.MainSnackbar.MessageQueue.Enqueue(subject.Name + " updated from " + PaperSource.CurrentPaperSource.Name), new CancellationTokenSource().Token, TaskCreationOptions.None, MainWindow.SyncContextTaskScheduler);
            //        }
            //    }
            //    PaperSource.SaveSubscription(Subscription, subscription);
            //}
        }
    }

    public static class PastPaperHelperUpdateService
    {
        public delegate void UpdateCompleteEventHandler(string status);

        public static event UpdateCompleteEventHandler UpdateCompleteEvent;


        public delegate void UpdateBeginEventHandler(string status);

        public static event UpdateBeginEventHandler UpdateBeginEvent;

        public static void OnUpdateComplete(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
