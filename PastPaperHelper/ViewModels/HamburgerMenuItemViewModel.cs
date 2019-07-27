namespace PastPaperHelper.ViewModels
{
    class HamburgerMenuItemViewModel : NotificationObject
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

        public HamburgerMenuItemViewModel() { }
        public HamburgerMenuItemViewModel(string title, object content)
        {
            _title = title;
            _content = content;
        }
    }
}
