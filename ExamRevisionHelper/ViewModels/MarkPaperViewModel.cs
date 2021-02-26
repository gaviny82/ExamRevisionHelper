using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ExamRevisionHelper.Core;
using ExamRevisionHelper.Core.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Spire.Pdf;
using Spire.Pdf.General.Find;

namespace ExamRevisionHelper.ViewModels
{
    public class MarkPaperViewModel : BindableBase, INavigationAware
    {
        public Variant MockPaper;

        /// <summary>
        /// true: wrong
        /// false: correct
        /// </summary>
        public ObservableCollection<QuestionMarkingViewModel> Questions = new ObservableCollection<QuestionMarkingViewModel>();

        #region Implement Prism.Regions.INavigationAware
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var param = navigationContext.Parameters;
            if (param.ContainsKey("MockPaper"))
            {
                MockPaper = param["MockPaper"] as Variant;

                int questions = GetNumberOfQuestions();
                Questions.Clear();
                for (int i = 1; i <= questions; i++)
                {
                    Questions.Add(new QuestionMarkingViewModel
                    {
                        QuestionNumber = i,
                        IsCorrect = true
                    });
                }

                foreach (Paper item in MockPaper.Papers)
                {
                    if (item.Type == ResourceType.MarkScheme)
                    {
                        var filename = item.Url?.Split('/').Last();
                        Process.Start(ExamRevisionHelperCore.LocalFiles[filename]);

                        Task.Run(() =>
                        {
                            try
                            {
                                PdfDocument doc = new PdfDocument(ExamRevisionHelperCore.LocalFiles[filename]);
                                PdfPageBase page = doc.Pages[0];
                                var match = page.FindText("Maximum Mark.?:.?\\d+", TextFindParameter.Regex).Finds.First();
                                string maxMarks = match?.MatchText.Split(':').Last().Trim();
                                int.TryParse(maxMarks, out int tmpmark);
                                MaxMarks = tmpmark;
                            }
                            catch (Exception)
                            {

                            }
                        });
                    }
                }
            }
        }
        #endregion

        private DelegateCommand _saveExamResultCommand;
        public DelegateCommand SaveExamResultCommand =>
            _saveExamResultCommand ?? (_saveExamResultCommand = new DelegateCommand(ExecuteSaveExamResultCommand));

        void ExecuteSaveExamResultCommand()
        {
            Paper questionPaper = null;
            foreach (var item in MockPaper.Papers)
            {
                if (item.Type == ResourceType.QuestionPaper) { questionPaper = item; break; }
            }
            PracticeExamData practiceExam = new PracticeExamData
            {
                Date = DateTime.Now,
                TotalMarks = MaxMarks,
                Mark = _yourMark == -1 ? 0 : _yourMark,
                QuestionPaper = questionPaper.Url.Split('/').Last(),
                Mistakes = (from item in Questions
                            where !item.IsCorrect
                            select item.QuestionNumber)
                            .ToArray(),
            };

            var subj = questionPaper.Exam.Subject;
            if (PracticeViewModel.MockExams.ContainsKey(subj))
            {
                var entries = PracticeViewModel.MockExams[subj].Append(practiceExam);
                PracticeViewModel.MockExams[subj] = entries.ToArray();
            }
            else
                PracticeViewModel.MockExams.Add(subj, new PracticeExamData[] { practiceExam });

            PracticeViewModel.SaveMockExamsData();
            (App.Current.MainWindow.DataContext as MainWindowViewModel).NavigateCommand.Execute("Practice");
        }

        private int _maxMarks;
        public int MaxMarks
        {
            get { return _maxMarks; }
            set { SetProperty(ref _maxMarks, value); }
        }

        private int _yourMark = -1;
        public string YourMark
        {
            get { return _yourMark == -1 ? "" : _yourMark.ToString(); }
            set { SetProperty(ref _yourMark, int.Parse(value)); }
        }

        private int GetNumberOfQuestions()
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
            if (qp == null) return 0;

            using PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(ExamRevisionHelperCore.LocalFiles[qp.Url.Split('/').Last()]);

            int pageIndex = doc.Pages.Count - 1;
            List<string> questionList = new List<string>();
            while (pageIndex >= 0 && questionList.Count == 0)
            {
                PdfPageBase page = doc.Pages[pageIndex--];
                string questions = page.ExtractText(new RectangleF(0, 0, 56, page.ActualSize.Height)).Replace("Evaluation Warning : The document was created with Spire.PDF for .NET.", "").Replace("\r", "");
                questionList = SearchViewModel.ProcessQuestionNumbers(questions);
            }
            if (questionList.Count == 0) return 0;

            bool parseResult = int.TryParse(questionList.Last(), out int num);
            return parseResult ? num : 0;
        }
    }

    public class QuestionMarkingViewModel : BindableBase
    {
        private int _questionNumber;
        public int QuestionNumber
        {
            get { return _questionNumber; }
            set { SetProperty(ref _questionNumber, value); }
        }

        private bool _isCorrect = true;
        public bool IsCorrect
        {
            get { return _isCorrect; }
            set { SetProperty(ref _isCorrect, value); }
        }
    }
}
