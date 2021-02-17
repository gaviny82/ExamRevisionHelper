using System;
using Windows.ApplicationModel.Core;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace ExamRevisionHelper
{
    public enum InitializationResult { SuccessNoUpdate, SuccessUpdateNeeded, Error }
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Windows.UI.Xaml.Controls.Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            //Default tab selected
            navView.SelectedItem = navView.MenuItems[2];

            // Hide default title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += (sender, e) => updateTitleBar(coreTitleBar, navView.DisplayMode, navView.IsPaneOpen);
            Window.Current.SetTitleBar(appTitleBar);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e == null) throw new ArgumentNullException(nameof(e));

            //Parse navigation parameters
            var (args, initTask) = ((string, Task<InitializationResult>))e.Parameter;
            InitializationResult initStatus = await initTask.ConfigureAwait(false);
            //TEST: sync/async
        }


        void updateTitleBar(CoreApplicationViewTitleBar coreTitleBar, NavigationViewDisplayMode mode, bool isPaneOpen)
        {
            double left = navView.CompactPaneLength;
            if (mode == NavigationViewDisplayMode.Minimal) left = left * 2 + 8;
            else
            {
                if (!isPaneOpen) left += 16;
            }
            appTitleBar.Margin = new Thickness(left, 0, coreTitleBar.SystemOverlayRightInset, 0);
        }

        private void navView_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
            => updateTitleBar(CoreApplication.GetCurrentView().TitleBar, args.DisplayMode, navView.IsPaneOpen);

        private void navView_PaneOpening(NavigationView sender, object args)
            => updateTitleBar(CoreApplication.GetCurrentView().TitleBar, navView.DisplayMode, true);

        private void navView_PaneClosing(NavigationView sender, NavigationViewPaneClosingEventArgs args)
            => updateTitleBar(CoreApplication.GetCurrentView().TitleBar, navView.DisplayMode, false);

        private void navView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                contentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                string selectedItemTag = (args.SelectedItem as NavigationViewItem).Tag as string;
                //sender.Header = "Sample Page " + selectedItemTag.Substring(selectedItemTag.Length - 1);
                
                Type pageType = Type.GetType($"ExamRevisionHelper.{selectedItemTag}");
                if (pageType == null) return;
                contentFrame.Navigate(pageType);
            }
        }
    }
}
