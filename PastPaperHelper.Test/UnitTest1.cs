using System;
using PastPaperHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PastPaperHelper.Sources;

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
    }
}
