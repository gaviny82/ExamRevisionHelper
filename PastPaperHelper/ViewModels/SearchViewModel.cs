using MaterialDesignThemes.Wpf;
using Spire.Pdf;
using Spire.Pdf.General.Find;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PastPaperHelper
{
    public class SearchViewModel : NotificationObject
    {
        CancellationTokenSource cts;
        ObservableCollection<Question> questions = new ObservableCollection<Question>();

        public SearchViewModel()
        {
            MatchWholeWord = false;
            IgnoreCases = true;
        }

        private SearchStatus _searchStatus;
        public SearchStatus SearchStatus
        {
            get { return _searchStatus; }
            set { _searchStatus = value; RaisePropertyChangedEvent("SearchStatus"); }
        }

        public IEnumerable<Question> Questions
        {
            get { return questions; }
        }

        private string _searchPath;
        public string SearchPath
        {
            get { return _searchPath; }
            set { _searchPath = value; RaisePropertyChangedEvent("SearchPath"); }
        }

        private string _keyword;
        public string Keyword
        {
            get { return _keyword; }
            set
            {
                _keyword = value;
                RaisePropertyChangedEvent("Keyword");
            }
        }

        private bool _ignoreCases;
        public bool IgnoreCases
        {
            get { return _ignoreCases; }
            set
            {
                _ignoreCases = value;
                RaisePropertyChangedEvent("IgnoreCases");
            }
        }

        private bool _matchWholeWord;
        public bool MatchWholeWord
        {
            get { return _matchWholeWord; }
            set
            {
                _matchWholeWord = value;
                RaisePropertyChangedEvent("MatchWholeWord");
            }
        }


        private double _progress;
        public double Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChangedEvent("Progress");
            }
        }

        private double _fileNum;
        public double FileNum
        {
            get { return _fileNum; }
            set
            {
                _fileNum = value;
                RaisePropertyChangedEvent("FileNum");
            }
        }
        
        private string _info = "Standby...";
        public string Info
        {
            get { return _info; }
            set
            {
                _info = value;
                RaisePropertyChangedEvent("Info");
            }
        }

        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null) _searchCommand = new DelegateCommand(Search);
                return _searchCommand;
            }
            private set
            {
                _searchCommand = value;
                RaisePropertyChangedEvent("SearchCommand");
            }
        }

        private void Cancel(object param)
        {
            cts.Cancel();
            Info = "Cancelled. Last file searched: " + Info.Substring(11);
            SearchCommand = new DelegateCommand(Search);
            SearchStatus = SearchStatus.Standby;
            GC.Collect();
        }
        private void Search(object param)
        {
            if (!Directory.Exists(SearchPath)) { DialogHost.OpenDialogCommand.Execute(Application.Current.MainWindow.FindResource("InvalidPath"), (IInputElement)param); return; }
            if (string.IsNullOrWhiteSpace(Keyword)) { DialogHost.OpenDialogCommand.Execute(Application.Current.MainWindow.FindResource("InvalidKeyword"), (IInputElement)param); return; }

            SearchStatus = SearchStatus.Searching;
            //search init
            string[] files = Directory.GetFiles(SearchPath, "*qp*.pdf", SearchOption.AllDirectories);
            SearchCommand = new DelegateCommand(Cancel);
            Progress = 0;
            FileNum = files.Length;
            questions.Clear();
            //start search tasks
            cts = new CancellationTokenSource();
            Task.Run(() => { Search(files, Keyword, MatchWholeWord, IgnoreCases); }, cts.Token);
        }
        private void Search(string[] files, string kword, bool wholeWord, bool ignCases)
        {
            foreach (string file in files)
            {
                List<int> fileResult = new List<int>();
                PdfDocument doc = new PdfDocument();
                doc.LoadFromFile(file);
                string fileName = file.Split('\\').Last();
                Progress += 1;
                Info = "Searching: " + fileName;
                for (int i = 1; i < doc.Pages.Count; i++)
                {
                    cts.Token.ThrowIfCancellationRequested();
                    PdfPageBase page = doc.Pages[i];
                    PdfTextFind[] coll = page.FindText(kword, wholeWord, ignCases).Finds;
                    foreach (PdfTextFind result in coll)
                    {
                        //extract the question numbers
                        RectangleF boundary = result.Bounds;
                        string questions = page.ExtractText(new RectangleF(0, 0, 56, boundary.Bottom)).Replace("Evaluation Warning : The document was created with Spire.PDF for .NET.", "").Replace("\r", "");
                        List<string> questionList = ProcessQuestionNumbers(questions);

                        //try to find the question number in previous pages if it is not found in this page
                        int pageIndex = i;
                        while (questionList.Count == 0 && pageIndex != 0)
                        {
                            PdfPageBase lastPage = doc.Pages[--pageIndex];
                            questions = lastPage.ExtractText(new RectangleF(0, 0, 56, lastPage.ActualSize.Height)).Replace("Evaluation Warning : The document was created with Spire.PDF for .NET.", "").Replace("\r", "");
                            questionList = ProcessQuestionNumbers(questions);
                        }

                        int questionNo;
                        if (questionList.Count == 0) questionNo = 0;
                        else int.TryParse(questionList.Last().ToString(), out questionNo);

                        //duplicate check
                        if (!fileResult.Contains(questionNo))
                        {
                            fileResult.Add(questionNo);
                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                SynchronizationContext.SetSynchronizationContext(new
                                  System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                                SynchronizationContext.Current.Post(pl =>
                                {
                                    this.questions.Add(new Question(fileName, questionNo) { FilePath = file });
                                }, null);
                            });
                        }
                    }
                }
                doc.Close();
            }
            SearchStatus = SearchStatus.Standby;
            SearchCommand = new DelegateCommand(Search);
            Info = "Done, " + questions.Count + " result" + (questions.Count > 1 ? "s" : "") + " found in " + FileNum + " files.";
        }
        private List<string> ProcessQuestionNumbers(string str)
        {
            List<string> list = str.Split('\n').ToList();
            for (int i = 0; i < list.Count; i++)
            {
                string item = list[i];
                string trimed = item.Trim();

                int index = 0;
                while (index < trimed.Length)
                {
                    if (trimed[index] != '0' && trimed[index] != '1' && trimed[index] != '2' && trimed[index] != '3' && trimed[index] != '4' && trimed[index] != '5' && trimed[index] != '6' && trimed[index] != '7' && trimed[index] != '8' && trimed[index] != '9')
                    {
                        break;
                    }
                    index++;
                }
                item = trimed.Substring(0, index);
                if (string.IsNullOrEmpty(item)) list.RemoveAt(i--);
            }

            return list;
        }

    }

    public enum SearchStatus { Standby, Searching }
}
