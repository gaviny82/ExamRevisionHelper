using ExamRevisionHelper.Core.Tools;
using ExamRevisionHelper.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Spire.Pdf;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;

namespace ExamRevisionHelper.ViewModels
{
    public class CountdownViewModel : BindableBase, INavigationAware
    {
        public Variant MockPaper;


        Timer timer = new Timer(1000) { AutoReset = true };
        public CountdownViewModel()
        {
            timer.Elapsed += (sender, e) =>
            {
                if (Countdown > 0) 
                    Countdown--;
                else if (Countdown == 0) 
                    timer.Stop();

            };
        }

        private bool _isMockModeOn = false;
        public bool IsMockModeOn
        {
            get { return _isMockModeOn; }
            set { SetProperty(ref _isMockModeOn, value); }
        }

        private bool _isMockExamStarted = false;
        public bool IsMockExamStarted
        {
            get { return _isMockExamStarted; }
            set { SetProperty(ref _isMockExamStarted, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string _mockExamControllerContent = "Start";
        public string MockExamControllerContent
        {
            get { return _mockExamControllerContent; }
            set { SetProperty(ref _mockExamControllerContent, value); }
        }

        private int _countdown;
        public int Countdown
        {
            get { return _countdown; }
            set { SetProperty(ref _countdown, value); }
        }

        #region ControlMockExamCommand
        private DelegateCommand _controlMockExamCommand;
        public DelegateCommand ControlMockExamCommand =>
            _controlMockExamCommand ?? (_controlMockExamCommand = new DelegateCommand(SwitchMockExamState));

        void SwitchMockExamState()
        {
            if (!IsMockExamStarted)
            {
                //Start a mock exam
                IsMockExamStarted = true;
                timer.Start();
                Message = "Exam started, time remaining:";
                MockExamControllerContent = "Finish";
            }
            else
            {
                //Finish exam
                IsMockExamStarted = false;
                timer.Stop();
                (App.Current.MainWindow.DataContext as MainWindowViewModel)
                .FinishMockExamCommand.Execute(MockPaper);
            }
        }
        #endregion


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
                IsMockModeOn = true;
                Message = "When your are ready for the exam, click the button below to start the timer.";
                MockPaper = navigationContext.Parameters["MockPaper"] as Variant;

                foreach (Paper item in MockPaper.Papers)
                {
                    var filename = item.Url?.Split('/').Last();
                    if (item.Type == ResourceType.QuestionPaper)
                    {
                        if (PastPaperHelperCore.LocalFiles.ContainsKey(filename))
                            Process.Start(PastPaperHelperCore.LocalFiles[filename]);
                        else
                            return;
                        using PdfDocument doc = new PdfDocument(PastPaperHelperCore.LocalFiles[filename]);
                        string txt = doc.Pages[0]?.ExtractText();
                        MatchCollection matches = Regex.Matches(txt, @"([0-9]+\s)(?:hour(s)?|minutes)((\s[0-9]+\s)(?:hour(s)?|minutes))*");

                        string match = matches[0]?.Value;
                        string[] timeData = matches[0]?.Value.Replace(" hours", "").Replace(" hour", "").Replace(" minutes", "").Split(' ');
                        
                        int minutes;
                        if (timeData.Length == 2)
                            minutes = int.Parse(timeData[0]) * 60 + int.Parse(timeData[1]);
                        else if (match.ToLower().Contains("hour"))
                            minutes = int.Parse(timeData[0]) * 60;
                        else
                            minutes = int.Parse(timeData[0]);
                        Countdown = minutes * 60;
                    }
                    else if (item.Type == ResourceType.Insert)
                    {
                        Process.Start(PastPaperHelperCore.LocalFiles[filename]);
                    }
                    else if (item.Type == ResourceType.ListeningAudio)
                    {
                        Process.Start(PastPaperHelperCore.LocalFiles[filename]);
                    }
                }
            }
        }
    }
}
