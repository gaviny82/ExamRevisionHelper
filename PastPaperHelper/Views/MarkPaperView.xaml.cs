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

namespace PastPaperHelper.Views
{
    /// <summary>
    /// MarkPaperView.xaml 的交互逻辑
    /// </summary>
    public partial class MarkPaperView : UserControl
    {
        public MarkPaperView()
        {
            InitializeComponent();
            list.ItemsSource = (DataContext as MarkPaperViewModel).Questions;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if((e.Key<Key.NumPad0||e.Key>Key.NumPad9) && (e.Key < Key.D0 || e.Key > Key.D9))
            {
                e.Handled = true;
            }
        }
    }
}
