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
            SubscriptionManager.CurrentPaperSource = PaperSources.GCE_Guide;
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
            PaperSource.SaveSubjectList(PaperSources.GCE_Guide.GetSubjectUrlMap(), doc);
            doc.Save(Environment.CurrentDirectory + "\\data\\subjects.xml");
        }
        [TestMethod]
        public void DownloadSubjectListTest()
        {
            var result = PaperSources.GCE_Guide.GetSubjectUrlMap();
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
            var result = PaperSources.GCE_Guide.GetPapers(subj, SubscriptionManager.SubjectUrlMap[subj]);
            Assert.IsNotNull(result.Exams);
            Assert.IsNotNull(result.Subject);
        }

        [TestMethod]
        public void SavePaperRepoTest()
        {
            SubscriptionManager.CurrentPaperSource = PaperSources.GCE_Guide;
            var phy = new Subject
            {
                Curriculum = Curriculums.ALevel,
                Name = "Physics",
                SyllabusCode = "9702"
            };
            var result = PaperSources.GCE_Guide.GetPapers(phy, @"https://papers.gceguide.com/A%20Levels/Physics%20(9702)/");
            Dictionary<Subject, PaperRepository> repo = new Dictionary<Subject, PaperRepository>();
            XmlDocument doc = new XmlDocument();
            repo.Add(phy, result);
            PaperSource.SaveSubscription(repo, doc);
            doc.Save(Environment.CurrentDirectory + "\\data\\physics.xml");
            Assert.IsNotNull(result.Exams);
            Assert.IsNotNull(result.Subject);
        }
    }
}
