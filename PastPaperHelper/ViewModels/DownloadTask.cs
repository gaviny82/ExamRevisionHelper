using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.ViewModels
{
    public enum DownloadTaskState { Pending, Completed, Error }
    public class DownloadTask : BindableBase
    {
        public string FileName { get; set; }

        #region Property: Progress
        private byte _progress;
        public byte Progress
        {
            get { return _progress; }
            set { SetProperty(ref _progress, value); }
        }
        #endregion

        #region Property: State
        private DownloadTaskState _state;
        public DownloadTaskState State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }
        #endregion

        public string ResourceUrl { get; set; }

        public string LocalPath { get; set; }
    }
}
