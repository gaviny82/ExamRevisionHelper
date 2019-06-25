using PastPaperHelper.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

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
