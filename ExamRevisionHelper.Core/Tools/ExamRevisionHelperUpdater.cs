using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Xml;
using ExamRevisionHelper.Core.Models;

namespace ExamRevisionHelper.Core
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

    public class SubjectUnsupportedException : Exception
    {
        public string[] UnsupportedSubjects { get; set; }

        public SubjectUnsupportedException() { }
        public SubjectUnsupportedException(string msg) : base(msg) { }
    }

    public class ExamRevisionHelperUpdater
    {
        public ExamRevisionHelperUpdater([NotNull]ExamRevisionHelperCore coreInstance)
        {
            _coreInstance = coreInstance;
        }

        private readonly ExamRevisionHelperCore _coreInstance;

        public delegate void UpdateServiceNotifiedEventHandler(UpdateServiceNotifiedEventArgs args);

        public event UpdateServiceNotifiedEventHandler UpdateServiceNotifiedEvent;


        public delegate void UpdateServiceErrorEventHandler(UpdateServiceErrorEventArgs args);

        public event UpdateServiceErrorEventHandler UpdateServiceErrorEvent;


        public delegate void SubjectUnsubscribedEventHandler(Subject subj);

        public event SubjectUnsubscribedEventHandler SubjectUnsubscribedEvent;


        public delegate void SubjectSubscribedEventHandler(Subject subj);

        public event SubjectSubscribedEventHandler SubjectSubscribedEvent;


        public delegate void SubjectSubscribeFailedEventHandler(Subject subj);

        public event SubjectSubscribeFailedEventHandler SubjectSubscribeFailedEvent;


        public async void UpdateAll(ICollection<string> subscribedSubjects)
        {
            var source = _coreInstance.CurrentSource;

            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
            {
                NotificationType = NotificationType.Initializing,
                Message = $"Loading from {source.DisplayName}..."
            });

            //Download subject list from web server
            try
            {
                await source.UpdateSubjectUrlMapAsync();
            }
            catch (Exception e)
            {
                UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
                {
                    ErrorMessage = $"Failed to download subject list from {source.DisplayName}, please check your Internet connection.",
                    ErrorType = ErrorType.SubjectListUpdateFailed,
                    Exception = e
                });
                return;
            }

            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
            {
                Message = $"Subject list updated from {source.DisplayName}.",
                NotificationType = NotificationType.SubjectListUpdated
            });

            //Download papers from web server
            Subject[] subjList = _coreInstance.SubjectsAvailable;
            List<string> failedList = new();
            foreach (var item in subscribedSubjects)
            {
                var found = ExamRevisionHelperCore.TryFindSubject(item, out _, subjList);
                if (!found) failedList.Add(item);
            }
            if (failedList.Count != 0) UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
            {
                ErrorMessage = $"Syllabus code: {string.Join(',', failedList)} is not supported.",
                ErrorType = ErrorType.SubjectNotSupported,
                Exception = new SubjectUnsupportedException($"Syllabus code: {string.Join(',', failedList)} is not supported.") { UnsupportedSubjects = failedList.ToArray() }
            });

            List<Subject> failed = new();
            foreach (Subject subj in _coreInstance.SubjectsSubscribed)
            {
                try
                {
                    await _coreInstance.AddOrUpdateSubject(subj);
                }
                catch (Exception e)
                {
                    failed.Add(subj);
                    UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
                    {
                        ErrorMessage = $"Failed to update {subj.Name} from {source.DisplayName}.",
                        ErrorType = ErrorType.SubjectRepoUpdateFailed,
                        Exception = e
                    });
                    continue;
                }

                UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
                {
                    Message = $"{subj.Name} updated from {source.DisplayName}.",
                    NotificationType = NotificationType.SubjectUpdated
                });
            }

            //Update (partially) failed
            if (failed.Count != 0) return;

            //Update successful
            XmlDocument dataDocument = source.SaveDataToXml(_coreInstance.SubscriptionRepo);
            _coreInstance.UserData = dataDocument;

            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
            {
                Message = $"All subjects updated from {source.DisplayName} successfully.",
                NotificationType = NotificationType.Finished
            });
        }

        public async Task UpdateSubjectList()
        {
            var source = _coreInstance.CurrentSource;

            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
            {
                NotificationType = NotificationType.Initializing,
                Message = $"Loading subject list from {source.DisplayName}..."
            });
            try
            {
                await source.UpdateSubjectUrlMapAsync();
            }
            catch (Exception e)
            {
                UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
                {
                    ErrorMessage = $"Failed to fetch data from {source.DisplayName}, please check your Internet connection.",
                    ErrorType = ErrorType.SubjectListUpdateFailed,
                    Exception = e
                });
                return;
            }
            UpdateServiceNotifiedEvent?.Invoke(new UpdateServiceNotifiedEventArgs
            {
                Message = $"Subject list updated from {source.DisplayName} successfully.",
                NotificationType = NotificationType.Finished
            });
        }

        public async Task<bool> SubscribeAsync(Subject subj)
        {
            try
            {
                if (_coreInstance.SubscriptionRepo.ContainsKey(subj)) return false;
                await _coreInstance.AddOrUpdateSubject(subj);
            }
            catch (Exception)
            {
                SubjectSubscribeFailedEvent?.Invoke(subj);
                return false;
            }
            SubjectSubscribedEvent?.Invoke(subj);
            return true;
        }

        public void Unsubscribe(Subject subject)
        {
            _coreInstance.SubscriptionRepo.Remove(subject);
            //TODO: Update XML cache, and make this async
            SubjectUnsubscribedEvent?.Invoke(subject);
        }
    }
}
