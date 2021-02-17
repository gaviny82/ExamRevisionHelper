using ExamRevisionHelper.Core.Tools;
using Prism.Events;

namespace ExamRevisionHelper.Events
{
    public class UpdateServiceNotifiedEvent : PubSubEvent<UpdateServiceNotifiedEventArgs> { }
    public class UpdateServiceErrorEvent : PubSubEvent<UpdateServiceErrorEventArgs> { }
}
