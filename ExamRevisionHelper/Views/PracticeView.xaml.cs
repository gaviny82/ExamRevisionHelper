using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ExamRevisionHelper.Core;
using ExamRevisionHelper.ViewModels;

namespace ExamRevisionHelper.Views
{
    /// <summary>
    /// PracticeView.xaml 的交互逻辑
    /// </summary>
    public partial class PracticeView : UserControl
    {
        public PracticeView()
        {
            InitializeComponent();
        }

        private void view_qp_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var vm = btn.DataContext as MistakeViewModel;
            Process.Start(PastPaperHelperCore.LocalFiles[vm.QuestionPaper]);
        }

        private void view_ms_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var vm = btn.DataContext as MistakeViewModel;
            Process.Start(PastPaperHelperCore.LocalFiles[vm.QuestionPaper.Replace("qp", "ms")]);
        }
    }
}
