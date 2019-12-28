using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            PaperSource.CurrentPaperSource = PaperSources.GCE_Guide;
            LoadRepoTest();
        }

        [TestMethod]
        public void LoadRepoTest()
        {
            SubscriptionManager.CheckUpdate(out bool a, out bool b);
            SubscriptionManager.UpdateAndInit(false, false);
        }

        [TestMethod]
        public void FetchSubjectListTest()
        {
            //Download test
            var subjList = PaperSource.CurrentPaperSource.GetSubjectUrlMap();
            Assert.IsNotNull(subjList);

            //Write to XML
            XmlDocument doc = new XmlDocument();
            PaperSource.SaveSubjectList(subjList, doc);
            doc.Save(Environment.CurrentDirectory + "\\data\\subjects.xml");
        }

        [TestMethod]
        public void DownloadPapersTest()
        {
            //Sample subject
            var subj = new Subject
            {
                Curriculum = Curriculums.IGCSE,
                Name = "Economics",
                SyllabusCode = "0455"
            };
            //Download all papers of the sample subject
            var result = PaperSource.CurrentPaperSource.GetPapers(subj, SubscriptionManager.SubjectUrlMap[subj]);
            Assert.IsNotNull(result);

            //Create a test repo
            Dictionary<Subject, PaperRepository> repo = new Dictionary<Subject, PaperRepository>
            {
                { subj, result }
            };
            //Write to XML
            XmlDocument doc = new XmlDocument();
            PaperSource.SaveSubscription(repo, doc);
            doc.Save(Environment.CurrentDirectory + "\\data\\subject.xml");
        }
    }
}
