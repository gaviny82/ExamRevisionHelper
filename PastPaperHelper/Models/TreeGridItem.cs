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
        public TreeGridItem(string title, string name, int age, string sex, int level)
        {
            this._jobTitle = title;
            this._sex = sex;
            this._age = age;
        }
        private string _name;
        private int _age;
        private string _sex;
        private int _level;
        private string _jobTitle;

        public string Sex { get { return this._sex; } set { this._sex = value; } }
        public int Age { get { return this._age; } set { this._age = value; } }
        public int Level
        {
            get
            {
                return this._level;
            }
            set
            {
                _level = value;
                RaisePropertyChangedEvent("Level");
            }
        }

        private ObservableCollection<TreeGridItem> _children = new ObservableCollection<TreeGridItem>();
        public ObservableCollection<TreeGridItem> Children
        {
            get { return _children; }
        }

    }
}
