using ExamRevisionHelper.Models;
using System;

namespace ExamRevisionHelper.Utils
{
    public delegate void UpdateServiceNotificationEventHandler(UpdateServiceNotificationEventArgs args);
    public delegate void UpdateServiceErrorEventHandler(UpdateServiceErrorEventArgs args);
    public delegate void SubjectUnsubscribedEventHandler(Subject subj);
    public delegate void SubjectSubscribedEventHandler(Subject subj);

    public enum ErrorType { SubjectListUpdateFailed, SubjectRepoUpdateFailed, SubjectNotSupported, Other }
    public enum NotificationType { Initializing, SubjectListUpdated, SubjectUpdated, Finished }

    public class UpdateServiceNotificationEventArgs : EventArgs
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

}
