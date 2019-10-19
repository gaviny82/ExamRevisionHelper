using PastPaperHelper.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PastPaperHelper.Models;

namespace PastPaperHelper.Views
{
    /// <summary>
    /// LocalStorage.xaml 的交互逻辑
    /// </summary>
    public partial class LocalStorage : Grid
    {
        public LocalStorage()
        {
            InitializeComponent();
            DataContext = new LocalStorageViewModel();
            //var item = new TreeGridItem { Age = 10, Name = "Name1", Sex = "Male" };
            //item.Children.Add(new TreeGridItem { Age = 12, Name = "Child1", Sex = "Female" });
            //item.Children.Add(new TreeGridItem { Age = 12, Name = "Child2", Sex = "Female" });
            //var item2 = new TreeGridItem { Age = 10, Name = "Name2", Sex = "Male" };
            //item2.Children.Add(new TreeGridItem { Age = 12, Name = "Child1", Sex = "Female" });
            //item2.Children.Add(new TreeGridItem { Age = 12, Name = "Child2", Sex = "Female" });

            //item.Children[0].Children.Add(new TreeGridItem { Age = 1, Name = "Level3" });
            //_list.Items.Add(item);
            //_list.Items.Add(item2);
        }
    }
}
