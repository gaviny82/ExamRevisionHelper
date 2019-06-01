using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper
{
    class HamburgerMenuItem : ObservableObject
    {
        private string _title;
        private object _content;

        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChangedEvent("Title"); }
        }
        public object Content
        {
            get { return _content; }
            set { _content = value; RaisePropertyChangedEvent("Content"); }
        }

        public HamburgerMenuItem() { }
        public HamburgerMenuItem(string title, object content)
        {
            _title = title;
            _content = content;
        }
    }
}
