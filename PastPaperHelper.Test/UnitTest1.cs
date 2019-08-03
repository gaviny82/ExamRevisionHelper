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
        [TestMethod]
        public void SaveSubjectListTest()
        {
            XmlDocument doc = new XmlDocument();
            SubscriptionManager.CurrentPaperSource = PaperSources.GCE_Guide;
            PaperSource.SaveSubjectList(PaperSources.GCE_Guide.GetSubjects(), doc);
            doc.Save(Environment.CurrentDirectory + "\\data\\subjects.xml");
        }
        [TestMethod]
        public void DownloadSubjectListTest()
        {
            var result = PaperSources.GCE_Guide.GetSubjects();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DownloadPapersTest()
        {
            var result = PaperSources.GCE_Guide.GetPapers(new SubjectSource
            {
                SubjectInfo = new Subject(Curriculums.IGCSE, "Economics", "0455"),
                Url = @"https://papers.gceguide.com/IGCSE/Economics%20(0455)/"
            });
            Assert.IsNotNull(result.Exams);
            Assert.IsNotNull(result.Subject);
        }

        [TestMethod]
        public void SavePaperRepoTest()
        {
            SubscriptionManager.CurrentPaperSource = PaperSources.GCE_Guide;
            var phy = new Subject(Curriculums.ALevel, "Physics", "9702");
            var result = PaperSources.GCE_Guide.GetPapers(new SubjectSource
            {
                SubjectInfo = phy,
                Url = @"https://papers.gceguide.com/A%20Levels/Physics%20(9702)/",
            });
            Dictionary<Subject, PaperRepository> repo = new Dictionary<Subject, PaperRepository>();
            XmlDocument doc = new XmlDocument();
            repo.Add(phy, result);
            PaperSource.SaveSubscription(repo, doc);
            doc.Save(Environment.CurrentDirectory + "\\data\\physics.xml");
            Assert.IsNotNull(result.Exams);
            Assert.IsNotNull(result.Subject);
        }

        [TestMethod]
        public void LoadRepoTest()
        {
            SubscriptionManager.CurrentPaperSource = PaperSources.GCE_Guide;
            SubscriptionManager.CheckUpdate(out bool a, out bool b);
            SubscriptionManager.UpdateAndInit(false, false);
        }
    }
}
