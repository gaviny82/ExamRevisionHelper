using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ExamRevisionHelper.Core;
using ExamRevisionHelper.Core.Models;
using ExamRevisionHelper.Core.Sources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PastPaperHelper.Test
{
    [TestClass]
    public class UnitTest1
    {
        ExamRevisionHelperCore Instance;
        PaperSource Source => Instance.CurrentSource;

        public UnitTest1()
        {
            var storage = new DirectoryInfo(Environment.CurrentDirectory);
            Instance = new(null, storage, UpdateFrequency.Always, new string[] { "0455", "9231" });
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
            Source.UpdateSubjectUrlMapAsync().Wait();
            Assert.IsNotNull(Source.SubjectUrlMap);

            //Write to XML
            XmlDocument doc = Source.SaveDataToXml(Instance.SubscriptionRepo);
            doc.Save(Environment.CurrentDirectory + "\\subjects_test.xml");
        }

        [TestMethod]
        public void DownloadPapersTest()
        {
            Source.UpdateSubjectUrlMapAsync().Wait();
            Assert.IsNotNull(Source.SubjectUrlMap);

            //Sample subject
            var subj = new Subject
            {
                Curriculum = Curriculums.IGCSE,
                Name = "Economics",
                SyllabusCode = "0455"
            };
            //Download all papers of the sample subject
            var result = Source.GetPapers(subj).GetAwaiter().GetResult();
            Assert.IsNotNull(result);

            //Create a test repo
            Dictionary<Subject, PaperRepository> repo = new Dictionary<Subject, PaperRepository>
            {
                { subj, result }
            };
            //Write to XML
            XmlDocument doc = Source.SaveDataToXml(repo);
            doc.Save(Environment.CurrentDirectory + "\\subscription_test.xml");
        }
    }
}
