using ExamRevisionHelper.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Xml;
using System.Collections.Specialized;

namespace ExamRevisionHelper.Core.Tools
{
    public enum ErrorType { SubjectListUpdateFailed, SubjectRepoUpdateFailed, SubjectNotSupported, Other }
    public enum NotificationType { Initializing, SubjectListUpdated, SubjectUpdated, Finished }

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

#pragma warning disable CA2237 // Mark ISerializable types with serializable
    public class SubjectUnsupportedException : Exception
    {
        public string[] UnsupportedSubjects { get; set; }

        public SubjectUnsupportedException() { }
        public SubjectUnsupportedException(string msg) : base(msg) { }
    }

#pragma warning restore CA2237 // Mark ISerializable types with serializable

    public static class PastPaperHelperUpdateService
    {
        public delegate void UpdateServiceNotifiedEventHandler(UpdateServiceNotifiedEventArgs args);

        public static event UpdateServiceNotifiedEventHandler UpdateServiceNotifiedEvent;


        public delegate void UpdateServiceErrorEventHandler(UpdateServiceErrorEventArgs args);

        public static event UpdateServiceErrorEventHandler UpdateServiceErrorEvent;


        public delegate void SubjectUnsubscribedEventHandler(Subject subj);

        public static event SubjectUnsubscribedEventHandler SubjectUnsubscribedEvent;


        public delegate void SubjectSubscribedEventHandler(Subject subj);

        public static event SubjectSubscribedEventHandler SubjectSubscribedEvent;


        public delegate void SubjectSubscribeFailedEventHandler(Subject subj);

        public static event SubjectSubscribeFailedEventHandler SubjectSubscribeFailedEvent;


        public static async void UpdateAll(ICollection<string> subscribedSubjects)
        {
            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs 
            { 
                NotificationType = NotificationType.Initializing, 
                Message = $"Loading from {PastPaperHelperCore.Source.DisplayName}..." 
            });

            //Download subject list from web server
            try
            {
                await PastPaperHelperCore.Source.UpdateSubjectUrlMapAsync();
                PastPaperHelperCore.SubjectsLoaded = PastPaperHelperCore.Source.SubjectUrlMap.Keys.ToArray();
            }
            catch (Exception e)
            {
                UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
                {
                    ErrorMessage = $"Failed to download subject list from {PastPaperHelperCore.Source.DisplayName}, please check your Internet connection.",
                    ErrorType = ErrorType.SubjectListUpdateFailed,
                    Exception= e
                });
                return;
            }

            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
            {
                Message = $"Subject list updated from {PastPaperHelperCore.Source.DisplayName}.",
                NotificationType = NotificationType.SubjectListUpdated
            });

            //Download papers from web server
            try
            {
                PastPaperHelperCore.LoadSubscribedSubjects(subscribedSubjects);
            }
            catch (SubjectUnsupportedException e)
            {
                UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
                {
                    ErrorMessage = e.Message,
                    ErrorType = ErrorType.SubjectNotSupported,
                    Exception = e
                });
            }

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
                        ErrorMessage = $"Failed to update {subj.Name} from {PastPaperHelperCore.Source.DisplayName}.",
                        ErrorType = ErrorType.SubjectRepoUpdateFailed,
                        Exception = e
                    });
                    continue;
                }

                UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
                {
                    Message = $"{subj.Name} updated from {PastPaperHelperCore.Source.DisplayName}.",
                    NotificationType = NotificationType.SubjectUpdated
                });
            }

            if (failed.Count != 0) return;
            try
            {
                //Update finished
                XmlDocument dataDocument = PastPaperHelperCore.Source.SaveDataToXml();
                dataDocument.Save(PastPaperHelperCore.UserDataPath);

                UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
                {
                    Message = $"All subjects updated from {PastPaperHelperCore.Source.DisplayName} successfully.",
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
                Message = $"Loading subject list from {PastPaperHelperCore.Source.DisplayName}..."
            });
            try
            {
                await PastPaperHelperCore.Source.UpdateSubjectUrlMapAsync();
                PastPaperHelperCore.SubjectsLoaded = PastPaperHelperCore.Source.SubjectUrlMap.Keys.ToArray();
            }
            catch (Exception e)
            {
                UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
                {
                    ErrorMessage = $"Failed to fetch data from {PastPaperHelperCore.Source.DisplayName}, please check your Internet connection.",
                    ErrorType = ErrorType.SubjectListUpdateFailed,
                    Exception = e
                });
                return;
            }
            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
            {
                Message = $"Subject list updated from {PastPaperHelperCore.Source.DisplayName} successfully.",
                NotificationType = NotificationType.Finished
            });
        }

        public async static Task<bool> SubscribeAsync(Subject subj)
        {
            try
            {
                if (PastPaperHelperCore.SubscribedSubjects.Contains(subj)) return false;

                await PastPaperHelperCore.Source?.AddOrUpdateSubject(subj);
                PastPaperHelperCore.SubscribedSubjects.Add(subj);
            }
            catch (Exception)
            {
                SubjectSubscribeFailedEvent?.Invoke(subj);
                return false;
            }
            SubjectSubscribedEvent?.Invoke(subj);
            return true;
        }

        public static void Unsubscribe(Subject subject)
        {
            PastPaperHelperCore.SubscribedSubjects.Remove(subject);
            var dict = PastPaperHelperCore.Source.Subscription;
            if (dict.ContainsKey(subject)) dict.Remove(subject);
            SubjectUnsubscribedEvent?.Invoke(subject);
        }
    }
}
