using System;
using PastPaperHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PastPaperHelper.Sources;
using PastPaperHelper.Models;

namespace PastPaperHelper.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            PaperSource.SaveSubjectList(PaperSources.GCE_Guide.GetSubjects(), Environment.CurrentDirectory + "\\subject_list.xml", "GCE Guide");
        }
        [TestMethod]
        public void TestMethod1_1()
        {
            var result = PaperSources.GCE_Guide.GetSubjects();
        }

        [TestMethod]
        public void TestMethod2()
        {
            var result = PaperSources.GCE_Guide.GetPapers(new SubjectSource { Url = @"https://papers.gceguide.com/IGCSE/Economics%20(0455)/" });
        }
    }
}
