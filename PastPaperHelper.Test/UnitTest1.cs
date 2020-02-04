using Microsoft.VisualStudio.TestTools.UnitTesting;
using PastPaperHelper.Core.Tools;
using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using System;
using System.Collections.Generic;
using System.Xml;

namespace PastPaperHelper.Test
{
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            PastPaperHelperCore.Source = new PaperSourceGCEGuide();
        }

        [TestMethod]
        public void LoadRepoTest()
        {
            //SubscriptionManager.CheckUpdate(out bool a, out bool b);
            //SubscriptionManager.UpdateAndInit(false, false);
        }

        [TestMethod]
        public void FetchSubjectListTest()
        {
            //Download test
            PastPaperHelperCore.Source.UpdateSubjectUrlMapAsync().Wait();
            Assert.IsNotNull(PastPaperHelperCore.Source.SubjectUrlMap);

            //Write to XML
            XmlDocument doc = PastPaperHelperCore.Source.SaveDataToXml(null);
            doc.Save(Environment.CurrentDirectory + "\\subjects_test.xml");
        }

        [TestMethod]
        public void DownloadPapersTest()
        {
            PastPaperHelperCore.Source.UpdateSubjectUrlMapAsync().Wait();
            Assert.IsNotNull(PastPaperHelperCore.Source.SubjectUrlMap);

            //Sample subject
            var subj = new Subject
            {
                Curriculum = Curriculums.IGCSE,
                Name = "Economics",
                SyllabusCode = "0455"
            };
            //Download all papers of the sample subject
            var result = PastPaperHelperCore.Source.GetPapers(subj).GetAwaiter().GetResult();
            Assert.IsNotNull(result);

            //Create a test repo
            Dictionary<Subject, PaperRepository> repo = new Dictionary<Subject, PaperRepository>
            {
                { subj, result }
            };
            //Write to XML
            XmlDocument doc = PastPaperHelperCore.Source.SaveDataToXml(repo);
            doc.Save(Environment.CurrentDirectory + "\\subscription_test.xml");
        }
    }
}
