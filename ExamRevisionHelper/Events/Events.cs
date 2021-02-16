using ExamRevisionHelper.Core.Tools;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRevisionHelper.Events
{
    public class UpdateServiceNotifiedEvent : PubSubEvent<UpdateServiceNotifiedEventArgs> { }
    public class UpdateServiceErrorEvent : PubSubEvent<UpdateServiceErrorEventArgs> { }
}
