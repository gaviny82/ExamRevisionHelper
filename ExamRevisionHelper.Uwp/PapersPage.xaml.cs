using ExamRevisionHelper.Models;
using ExamRevisionHelper.ViewModels;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ExamRevisionHelper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PapersPage : Page
    {
        private readonly PapersPageViewModel VM = new PapersPageViewModel();

        public PapersPage()
        {
            this.InitializeComponent();
            DataContext = VM;
        }


        private void viewPaper_Click(object sender, RoutedEventArgs e)
        {
            string path = ((sender as Button).DataContext as Paper).Url;
            _ = Launcher.LaunchUriAsync(new System.Uri(path));
            //foreach (var file in MainWindowViewModel.Files)
            //{
            //    if (file.Split('\\').Last() == path.Split('/').Last())
            //    {
            //        path = file;
            //        break;
            //    }
            //}
            //Process.Start(path);
        }


        private void ItemsRepeater_ElementPrepared(Microsoft.UI.Xaml.Controls.ItemsRepeater sender, Microsoft.UI.Xaml.Controls.ItemsRepeaterElementPreparedEventArgs args)
        {
            double maxWidth = 0;

            int count = VisualTreeHelper.GetChildrenCount(sender);
            for (int i = 0; i < count; i++)
            {
                var panel = VisualTreeHelper.GetChild(sender, i) as StackPanel;
                if (panel != null && panel.ActualWidth > maxWidth) 
                    maxWidth = panel.ActualWidth;
            }
            sender.Width = maxWidth;
        }

        public void BindBack(string s) { }
    }
}
