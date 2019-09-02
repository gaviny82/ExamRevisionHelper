using PastPaperHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.ViewModels
{
    class SubjectViewModel : NotificationObject
    {
        public Subject Subject { get; set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; RaisePropertyChangedEvent("IsLoading"); }
        }

    }
}
