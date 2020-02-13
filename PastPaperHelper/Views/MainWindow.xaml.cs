using MaterialDesignThemes.Wpf;
using PastPaperHelper.Core.Tools;
using PastPaperHelper.Events;
using PastPaperHelper.ViewModels;
using Prism.Events;
using Prism.Regions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace PastPaperHelper.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Snackbar MainSnackbar { get; internal set; }
        public static TaskScheduler SyncContextTaskScheduler { get; internal set; }

        public MainWindow(IEventAggregator eventAggregator)
        {
            SyncContextTaskScheduler = TaskScheduler.Current;
            InitializeComponent();
            MainSnackbar = mainSnackbar;

            PastPaperHelperUpdateService.UpdateServiceNotifiedEvent += (args) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (args.NotificationType == NotificationType.Initializing)
                    {
                        Application.Current.Resources["IsLoading"] = Visibility.Visible;
                    }
                    else if (args.NotificationType == NotificationType.Finished)
                    {
                        Application.Current.Resources["IsLoading"] = Visibility.Hidden;
                        //TODO: SettingsViewModel.RefreshSubjectLists();
                        //TODO: SettingsViewModel.RefreshSubscription();
                    }
                    mainSnackbar.MessageQueue.Enqueue(args.Message);
                });
            };
            PastPaperHelperUpdateService.UpdateServiceErrorEvent += (args) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.Resources["IsLoading"] = Visibility.Hidden;
                    mainSnackbar.MessageQueue.Enqueue(args.ErrorMessage, "Retry", PastPaperHelperUpdateService.UpdateAll);
                });
            };

            InitializationResult initResult = (Application.Current as App).InitResult;
            if (initResult == InitializationResult.SuccessUpdateNeeded)
            {
                mainSnackbar.MessageQueue.Enqueue(
                    content: $"Update needed. (Last updated: {PastPaperHelperCore.Source.LastUpdated.ToShortDateString()})",
                    actionContent: "UPDATE", 
                    actionHandler: (param) => { PastPaperHelperUpdateService.UpdateAll(); }, null,
                    promote: true,
                    neverConsiderToBeDuplicate: true,
                    durationOverride:TimeSpan.FromSeconds(5));
            }
            else if (initResult == InitializationResult.Error)
            {
                mainSnackbar.MessageQueue.Enqueue(
                    content: $"An error has occurred. Try reloading from {PastPaperHelperCore.Source.Name}",
                    actionContent: "RELOAD",
                    actionHandler: (param)=> { PastPaperHelperUpdateService.UpdateAll(); }, null,
                    promote: true,
                    neverConsiderToBeDuplicate: true,
                    durationOverride: TimeSpan.FromDays(1));
            }
            else
            {
                //Refresh view models
                //TODO: Test needed
            }
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }
            MenuToggleButton.IsChecked = false;
        }

        private void RootDlg_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width, height = e.NewSize.Height;
            dlg.Width = width - 350;
            dlg.Height = height - 150;
        }

        private void HamburgerMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string uri = HamburgerMenu.SelectedItem?.ToString();
            if (uri != null) (DataContext as MainWindowViewModel).NavigateCommand.Execute(uri);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel).NavigateCommand.Execute(HamburgerMenu.SelectedItem.ToString().Replace(" ", ""));
        }

        private void Reference_Click(object sender, RoutedEventArgs e)
        {
            HamburgerMenu.SelectedItem = null;
            MenuToggleButton.IsChecked = false;
        }
    }
}
