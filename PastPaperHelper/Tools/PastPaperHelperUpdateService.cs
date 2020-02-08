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
    public enum ErrorType { SubjectListUpdateFailed, SubjectRepoUpdateFailed, Other }
    public enum NotificationType { Initializing, IntermediateTaskState, Finished }

    public class UpdateServiceNotifiedEventArgs : EventArgs
    {
        public NotificationType NotificationType { get; set; }
        public string Message { get; set; }
    }
    public class UpdateServiceErrorEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
        public ErrorType ErrorType { get; set; }
        public string ErrorMessage { get; set; }
    }


    public static class PastPaperHelperUpdateService
    {
        public delegate void UpdateServiceNotifiedEventHandler(UpdateServiceNotifiedEventArgs args);

        public static event UpdateServiceNotifiedEventHandler UpdateServiceNotifiedEvent;


        public delegate void UpdateServiceErrorEventHandler(UpdateServiceErrorEventArgs args);

        public static event UpdateServiceErrorEventHandler UpdateServiceErrorEvent;


        public static async void UpdateAll()
        {
            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs 
            { 
                NotificationType = NotificationType.Initializing, 
                Message = $"Loading from {PastPaperHelperCore.Source.Name}.../" 
            });

            //Download subject list from web server
            try
            {
                await PastPaperHelperCore.Source.UpdateSubjectUrlMapAsync();
                PastPaperHelperCore.UpdateSubjectLoaded();
            }
            catch (Exception e)
            {
                UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
                {
                    ErrorMessage = $"Failed to download subject list from {PastPaperHelperCore.Source.Name}, please check your Internet connection.",
                    ErrorType = ErrorType.SubjectListUpdateFailed,
                    Exception= e
                });
                return;
            }

            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
            {
                Message = $"Subject list updated from {PastPaperHelperCore.Source.Name}.",
                NotificationType = NotificationType.IntermediateTaskState
            });

            //Download papers from web server
            List<Subject> failed = new List<Subject>();
            foreach (Subject subj in PastPaperHelperCore.SubscribedSubjects)
            {
                try
                {
                    await PastPaperHelperCore.Source.AddOrUpdateSubject(subj);
                }
                catch (Exception e)
                {
                    failed.Add(subj);
                    UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
                    {
                        ErrorMessage = $"Failed to update {subj.Name} from {PastPaperHelperCore.Source.Name}.",
                        ErrorType = ErrorType.SubjectRepoUpdateFailed,
                        Exception = e
                    });
                    continue;
                }

                UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
                {
                    Message = $"{subj.Name} updated from {PastPaperHelperCore.Source.Name}.",
                    NotificationType = NotificationType.IntermediateTaskState
                });
            }

            if (failed.Count != 0) return;
            try
            {
                //Update finished
                XmlDocument dataDocument = PastPaperHelperCore.Source.SaveDataToXml(PastPaperHelperCore.Source.Subscription);
                dataDocument.Save(PastPaperHelperCore.UserDataPath);

                UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
                {
                    Message = $"All subjects updated from {PastPaperHelperCore.Source.Name} successfully.",
                    NotificationType = NotificationType.Finished
                });
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to save data to {PastPaperHelperCore.UserDataPath}", e);
            }
        }

        public static async Task UpdateSubjectList()
        {
            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
            {
                NotificationType = NotificationType.Initializing,
                Message = $"Loading subject list from {PastPaperHelperCore.Source.Name}.../"
            });
            try
            {
                await PastPaperHelperCore.Source.UpdateSubjectUrlMapAsync();
                PastPaperHelperCore.UpdateSubjectLoaded();
            }
            catch (Exception e)
            {
                UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
                {
                    ErrorMessage = $"Failed to fetch data from {PastPaperHelperCore.Source.Name}, please check your Internet connection.",
                    ErrorType = ErrorType.SubjectListUpdateFailed,
                    Exception = e
                });
                return;
            }
            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
            {
                Message = $"Subject list updated from {PastPaperHelperCore.Source.Name} successfully.",
                NotificationType = NotificationType.Finished
            });
        }

        /* TODO: Hot reload on update
        public static async Task UpdateSubject(Subject subj)
        {
            var downloadThread = Task.Run(() =>
            {
                return PastPaperHelperCore.Source.GetPapers(subj);
            });
            //Save to XML
            PastPaperHelperCore.Source.Subscription.Add(subj, await downloadThread);
        }
        */

    }
}
