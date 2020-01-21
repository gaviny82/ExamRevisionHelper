using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.ViewModels
{
    public enum DownloadTaskState { Pending, Completed, Error }
    public class DownloadTask : NotificationObject
    {
        public string FileName { get; set; }

        private byte _progress;
        public byte Progress
        {
            get { return _progress; }
            set { _progress = value; RaisePropertyChangedEvent(nameof(Progress)); }
        }

        private DownloadTaskState _state;
        public DownloadTaskState State
        {
            get { return _state; }
            set { _state = value; RaisePropertyChangedEvent(nameof(State)); }
        }

        public string ResourceUrl { get; set; }

        public string LocalPath { get; set; }
    }
}
