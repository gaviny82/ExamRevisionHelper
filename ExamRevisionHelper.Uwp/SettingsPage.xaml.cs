using ExamRevisionHelper.Dialogs;
using ExamRevisionHelper.Models;
using ExamRevisionHelper.Utils;
using ExamRevisionHelper.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ExamRevisionHelper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsPage : Page
    {

        readonly SettingsPageViewModel VM = new SettingsPageViewModel();

        public SettingsPage()
        {
            InitializeComponent();
            DataContext = VM;

            SubjectSubscriptionUtils.UpdateServiceNotificationEvent += (args) =>
            {
                var states = VisualStateManager.GetVisualStateGroups(this).ToList();
                if (args.NotificationType == NotificationType.Initializing)
                    VisualStateManager.GoToState(this, "Loading", false);
                else if (args.NotificationType == NotificationType.Finished)
                    VisualStateManager.GoToState(this, "Normal", false);
            };
        }

        private void ViewOnGithub_Click(object sender, RoutedEventArgs e)
        => _ = Launcher.LaunchUriAsync(new Uri("https://github.com/GavinYou082/ExamRevisionHelper"));

        private void ViewChangelog_Click(object sender, RoutedEventArgs e)
        {
            ChangelogDialog dialog = new ChangelogDialog();
            _ = dialog.ShowAsync();
        }

        private void ShowCreditsDialog_Click(object sender, RoutedEventArgs e)
        {
            //TODO: credits
        }

        private void OpenSubjectDialog_Click(object sender, RoutedEventArgs e)
        {
            SubjectDialog dialog = new SubjectDialog();
            _ = dialog.ShowAsync();
        }

        private void ItemContainer_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            (grid.Children.Last() as Button).Visibility = Visibility.Visible;
        }

        private void ItemContainer_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            (grid.Children.Last() as Button).Visibility = Visibility.Collapsed;
        }

        private void RemoveSelectedSubjects_Click(object sender, RoutedEventArgs e)
        {
            var list = subscribedSubjectList.SelectedItems;
            VM.UnsubscribeSelectedSubjectsCommand.Execute(list.ToArray());//ToArray copies the list
        }
    }
}
