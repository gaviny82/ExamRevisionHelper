using PastPaperHelper.Core.Tools;
using PastPaperHelper.Models;
using Prism.Mvvm;
using Prism.Regions;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.ViewModels
{
    public class MarkPaperViewModel : BindableBase, INavigationAware
    {
        public Variant MockPaper;

        #region Implement Prism.Regions.INavigationAware
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var param = navigationContext.Parameters;
            if (param.ContainsKey("MockPaper"))
            {
                MockPaper = param["MockPaper"] as Variant;
                AnalyzePaperStructure();
            }
        }
        #endregion

        private void AnalyzePaperStructure()
        {
            Paper qp = null;
            foreach (var item in MockPaper.Papers)
            {
                if (item.Type == ResourceType.QuestionPaper)
                {
                    qp = item;
                    break;
                }
            }
            if (qp == null) return;

            using PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(PastPaperHelperCore.LocalFiles[qp.Url.Split('/').Last()]);

            int pageIndex = doc.Pages.Count - 1;
            List<string> questionList = new List<string>();
            while (pageIndex >= 0 && questionList.Count == 0)
            {
                PdfPageBase page = doc.Pages[pageIndex--];
                string questions = page.ExtractText(new RectangleF(0, 0, 56, page.ActualSize.Height)).Replace("Evaluation Warning : The document was created with Spire.PDF for .NET.", "").Replace("\r", "");
                questionList = SearchViewModel.ProcessQuestionNumbers(questions);
            }
            if (questionList.Count == 0) return;


        }
    }
}
