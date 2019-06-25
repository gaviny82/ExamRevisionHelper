using System.ComponentModel;
using System.Windows.Threading;

namespace PastPaperHelper
{
    public abstract class NotificationObject : INotifyPropertyChanged
    {
        public Dispatcher Dispatcher { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
