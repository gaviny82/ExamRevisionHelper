using PastPaperHelper.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.Models
{
    class TreeGridItem : NotificationObject
    {
        public string Name { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }
        public ObservableCollection<TreeGridItem> Children { get; } = new ObservableCollection<TreeGridItem>();

    }
}
