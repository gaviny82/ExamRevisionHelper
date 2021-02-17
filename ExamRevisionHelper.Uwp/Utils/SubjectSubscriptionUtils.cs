using ExamRevisionHelper.Models;
using ExamRevisionHelper.Sources;
using ExamRevisionHelper.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;

namespace ExamRevisionHelper
{
    public static class SubjectSubscriptionUtils
    {
        public static event UpdateServiceNotificationEventHandler UpdateServiceNotificationEvent;
        public static event UpdateServiceErrorEventHandler UpdateServiceErrorEvent;
        public static event SubjectUnsubscribedEventHandler SubjectUnsubscribedEvent;
        public static event SubjectSubscribedEventHandler SubjectSubscribedEvent;

        public static async Task UpdateDataAsync(PaperSource source, ICollection<string> subscribedSubjects = null)
        {
            var paperSource = App.PaperSource;
            await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UpdateServiceNotificationEvent?.Invoke(new UpdateServiceNotificationEventArgs
                {
                    NotificationType = NotificationType.Initializing,
                    Message = $"Loading from {paperSource.DisplayName}..."
                });
            });

            //Download subject list from web server
            try
            {
                await paperSource.UpdateSubjectUrlMapAsync().ConfigureAwait(false);
                App.SubjectsLoaded = paperSource.SubjectUrlMap.Keys.ToArray();
            }
            catch (Exception e)
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
                    {
                        ErrorMessage = $"Failed to download subject list from {paperSource.DisplayName}, please check your Internet connection.",
                        ErrorType = ErrorType.SubjectListUpdateFailed,
                        Exception = e
                    });
                });
                return;
            }

            await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UpdateServiceNotificationEvent?.Invoke(new UpdateServiceNotificationEventArgs
                {
                    Message = $"Subject list updated from {paperSource.DisplayName}.",
                    NotificationType = NotificationType.SubjectListUpdated
                });
            });

            //Download papers from web server
            try
            {
                ReloadSubscribedSubjects(subscribedSubjects);
            }
            catch (SubjectUnsupportedException e)
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
                    {
                        ErrorMessage = e.Message,
                        ErrorType = ErrorType.SubjectNotSupported,
                        Exception = e
                    });
                });
            }

            List<Subject> failed = new List<Subject>();
            foreach (Subject subj in App.SubscribedSubjects)
            {
                try
                {
                    await paperSource.AddOrUpdateSubject(subj).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    failed.Add(subj);
                    await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
                        {
                            ErrorMessage = $"Failed to update {subj.Name} from {paperSource.DisplayName}.",
                            ErrorType = ErrorType.SubjectRepoUpdateFailed,
                            Exception = e
                        });
                    });
                    continue;
                }

                await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    UpdateServiceNotificationEvent?.Invoke(new UpdateServiceNotificationEventArgs
                    {
                        Message = $"{subj.Name} updated from {paperSource.DisplayName}.",
                        NotificationType = NotificationType.SubjectUpdated
                    });
                });
            }

            await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UpdateServiceNotificationEvent?.Invoke(new UpdateServiceNotificationEventArgs
                {
                    Message = $"Subjects updated from {paperSource.DisplayName} successfully, {failed.Count} failed",
                    NotificationType = NotificationType.Finished
                });
            });
            if (failed.Count != 0) return;

            try
            {
                //Update finished
                XmlDocument dataDocument = paperSource.SaveDataToXml();
                using (Stream outputStream = App.SourceDataFile.OpenStreamForWriteAsync().GetAwaiter().GetResult())
                {
                    dataDocument.Save(outputStream);
                }
                await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    UpdateServiceNotificationEvent?.Invoke(new UpdateServiceNotificationEventArgs
                    {
                        Message = $"All subjects updated from {paperSource.DisplayName} successfully.",
                        NotificationType = NotificationType.Finished
                    });
                });
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to save data to {App.SourceDataFile?.Path}", e);
            }
        }

        public async static Task<bool> SubscribeAsync(Subject subject)
        {
            if (subject == null) throw new ArgumentNullException(nameof(subject));

            try
            {
                if (App.SubscribedSubjects.Contains(subject)) return false;
                App.SubscribedSubjects.Add(subject);
                UpdateSetting();

                await (App.PaperSource?.AddOrUpdateSubject(subject)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                //Failed to subscribe a subject
                await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    UpdateServiceErrorEvent?.Invoke(new UpdateServiceErrorEventArgs
                    {
                        Exception = e,
                        ErrorMessage = $"Failed to subscribe {subject.Curriculum} {subject.Name}",
                    });
                });
                return false;
            }

            //Subject added to subscription
            await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                SubjectSubscribedEvent?.Invoke(subject);
            });
            return true;
        }

        public static void Unsubscribe(Subject subject)
        {
            if (subject == null) throw new ArgumentNullException(nameof(subject));

            App.SubscribedSubjects.Remove(subject);
            var dict = App.PaperSource.Subscription;
            if (dict.ContainsKey(subject)) dict.Remove(subject);
            UpdateSetting();

            SubjectUnsubscribedEvent?.Invoke(subject);
        }
        private static void UpdateSetting()
        {
            App.RoamingSettings.Values["SubjectsSubscribed"] = string.Join(',', App.SubscribedSubjects.Select(s => s.SyllabusCode));
        }


        public static void ReloadSubscribedSubjects(IEnumerable<string> subscription)
        {
            var subscribedSubjects = App.SubscribedSubjects;
            var subjectsLoaded = App.SubjectsLoaded;

            List<string> failed = new List<string>();
            subscribedSubjects.Clear();
            foreach (string item in subscription)
            {
                if (!TryFindSubject(item, out Subject subj) || !subjectsLoaded.Contains(subj))
                {
                    failed.Add(item);
                }
                else subscribedSubjects.Add(subj);
            }
            if (failed.Count != 0)
            {
                string code = "";
                failed.ForEach((str) => { code += str + ","; });
                throw new SubjectUnsupportedException($"{code.Substring(0, code.Length - 1)} not supported") { UnsupportedSubjects = failed.ToArray() };
            }
        }

        public static bool TryFindSubject(string syllabusCode, out Subject result, IEnumerable<Subject> range = null)
        {
            foreach (Subject item in range ?? App.SubjectsLoaded)
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
