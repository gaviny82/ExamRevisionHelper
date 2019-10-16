using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PastPaperHelper.Sources;
using PastPaperHelper.Models;
using System.Xml;
using System.Collections.Generic;

namespace PastPaperHelper.Test
{
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            PaperSource.CurrentPaperSource = PaperSources.PapaCambridge;
            LoadRepoTest();
        }

        [TestMethod]
        public void LoadRepoTest()
        {
            SubscriptionManager.CheckUpdate(out bool a, out bool b);
            SubscriptionManager.UpdateAndInit(false, false);
        }

        [TestMethod]
        public void SaveSubjectListTest()
        {
            XmlDocument doc = new XmlDocument();
            PaperSource.SaveSubjectList(PaperSources.PapaCambridge.GetSubjectUrlMap(), doc);
            doc.Save(Environment.CurrentDirectory + "\\data\\subjects.xml");
        }
        [TestMethod]
        public void DownloadSubjectListTest()
        {
            var result = PaperSource.CurrentPaperSource.GetSubjectUrlMap();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DownloadPapersTest()
        {
            var subj = new Subject
            {
                Curriculum = Curriculums.IGCSE,
                Name = "Economics",
                SyllabusCode = "0455"
            };
            var result = PaperSource.CurrentPaperSource.GetPapers(subj, SubscriptionManager.SubjectUrlMap[subj]);
            Dictionary<Subject, PaperRepository> repo = new Dictionary<Subject, PaperRepository>();
            XmlDocument doc = new XmlDocument();
            repo.Add(subj, result);
            PaperSource.SaveSubscription(repo, doc);
            Assert.IsNotNull(result);
        }
    }
}
