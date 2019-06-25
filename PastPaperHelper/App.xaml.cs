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
        public static SubjectSource[] AllSubjects;
        public static Dictionary<SubjectSource, PaperItem[]> PaperDictionary = new Dictionary<SubjectSource, PaperItem[]>();

        static App()
        {
            
        }
    }
}
