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
        public static XmlDocument SubjectList;

        static App()
        {
            if (File.Exists(Environment.CurrentDirectory + "\\subject_list.xml"))
            {
                XmlDocument doc = new XmlDocument();
                SubjectList = doc;
                doc.Load(Environment.CurrentDirectory + "\\subject_list.xml");
            }
        }
    }
}
