using System;
using PastPaperHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PastPaperHelper.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            SourceManager.UpdateSubjectsFromSource(PastPaperSources.GCEGuide);
        }
    }
}
