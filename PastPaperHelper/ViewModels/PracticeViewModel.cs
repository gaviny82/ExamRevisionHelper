using PastPaperHelper.Core.Tools;
using PastPaperHelper.Models;
using Prism.Mvvm;
using Prism.Regions;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace PastPaperHelper.ViewModels
{
    public class PracticeViewModel : BindableBase, INavigationAware
    {
        public static Variant MockPaper;

        private bool _isMockStarted = false;
        public bool IsMockStarted
        {
            get { return _isMockStarted; }
            set { SetProperty(ref _isMockStarted, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private TimeSpan _countdown;
        public TimeSpan Countdown
        {
            get { return _countdown; }
            set { SetProperty(ref _countdown, value); }
        }

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
                IsMockStarted = true;
                Message = "When your are ready for the exam, click the button below to start the timer.";
                MockPaper = navigationContext.Parameters["MockPaper"] as Variant;

                foreach (Paper item in MockPaper.Papers)
                {
                    var filename = item.Url?.Split('/').Last();
                    if (item.Type == ResourceType.QuestionPaper)
                    {
                        Process.Start(PastPaperHelperCore.LocalFiles[filename]);
                        using PdfDocument doc = new PdfDocument(PastPaperHelperCore.LocalFiles[filename]);
                        string txt = doc.Pages[0]?.ExtractText();
                        MatchCollection matches = Regex.Matches(txt, @"([0-9]+\s)(?:hour(s)?|minutes)((\s[0-9]+\s)(?:hour(s)?|minutes))*");
                        string[] timeData = matches[0]?.Value.Replace(" hours", "").Replace(" hour", "").Replace(" minutes","").Split(' ');
                        Countdown = new TimeSpan();

                        Timer timer = new Timer();
                        
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
