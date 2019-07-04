using PastPaperHelper.Models;
using System.Collections.Generic;
using System.Windows;

namespace PastPaperHelper
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static SubjectSource[] _allSubjects;
        public static SubjectSource[] AllSubjects
        {
            get { return _allSubjects; }
            set
            {
                _allSubjects = value;
                DownloadViewModel.UpdateSubjectList();
            }
        }
        public static Dictionary<SubjectSource, PaperItem[]> SubscriptionDict = new Dictionary<SubjectSource, PaperItem[]>();

        static App()
        {

        }
    }
}
