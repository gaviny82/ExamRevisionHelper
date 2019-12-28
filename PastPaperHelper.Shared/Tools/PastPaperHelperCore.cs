using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PastPaperHelper.Core.Tools
{
    public enum InitializeStatus { NoUpdate, Updated, UpdateFailed }
    public static class PastPaperHelperCore
    {
        public static Subject[] SubjectsLoaded { get; set; }
        public static Dictionary<Subject, PaperRepository> Subscription { get; set; }
        private static PaperSource CurrentSource { get; set; }

        private static XmlDocument data;

        public static async Task<InitializeStatus> Initialize(PaperSource source, XmlDocument data, DateTime lastUpdate, UpdatePolicy updatePolicy, string[] subscription)
        {
            CurrentSource = source;
            PastPaperHelperCore.data = data;

            bool needUpdate = false;
            if (data == null) needUpdate = true;
            else
            {
                XmlNode updateInfo = data.SelectSingleNode("/UpdateInfo");
                if (updateInfo == null||updateInfo.Attributes["LastUpdate"]==null) needUpdate = true;
                else
                {
                    DateTime.TryParse( updateInfo.Attributes["LastUpdate"].Value, out lastUpdate);
                    if ((DateTime.Now - lastUpdate).TotalDays > 100) needUpdate = true;//TODO: set update frequency
                }
            }
            Update();
        }

        public static async Task Update(Subject[] subscription)
        {
            Dictionary<Subject,string> subjUrlMap = CurrentSource.GetSubjectUrlMap();
            SubjectsLoaded = subjUrlMap.Keys.ToArray();
            foreach (Subject subj in subscription)
            {
                Subscription.Add(subj, CurrentSource.GetPapers(subj));
            }
        }

        public static void Subscribe(Subject subject)
        {

        }

        public static void Unsubscribe(Subject subject)
        {
           
        }

    }
}
