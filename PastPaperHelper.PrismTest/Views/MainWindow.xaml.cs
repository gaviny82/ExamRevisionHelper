using MaterialDesignThemes.Wpf;
using PastPaperHelper.Core.Tools;
using PastPaperHelper.PrismTest.Events;
using PastPaperHelper.PrismTest.ViewModels;
using Prism.Events;
using Prism.Regions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace PastPaperHelper.PrismTest.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Snackbar MainSnackbar { get; internal set; }
        public static TaskScheduler SyncContextTaskScheduler { get; internal set; }

        private readonly IRegionManager _regionManager;
        public MainWindow(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            InitializeComponent();
            MainSnackbar = mainSnackbar;
            SyncContextTaskScheduler = TaskScheduler.Current;
            _regionManager = regionManager;


            InitializationResult initResult = (Application.Current as App).InitResult;
            if (initResult == InitializationResult.SuccessUpdateNeeded)
            {
                //TODO: Test needed
                mainSnackbar.MessageQueue.Enqueue($"Last update: {PastPaperHelperCore.LastUpdated.ToLongDateString()}", "Update", () =>
                {
                    Application.Current.Resources["IsLoading"] = Visibility.Visible;
                    PastPaperHelperUpdateService.UpdateAll(Properties.Settings.Default.SubjectsSubcription);
                });
                //Refresh view models
            }
            else if (initResult == InitializationResult.Error)
            {
                PastPaperHelperUpdateService.UpdateInitiatedEvent += delegate
                {
                    eventAggregator.GetEvent<MessageBarEnqueuedEvent>().Publish($"Fetching data from {PastPaperHelperCore.CurrentSource.Name}...");
                    Application.Current.Resources["IsLoading"] = Visibility.Visible;
                };
                PastPaperHelperUpdateService.UpdateErrorEvent += (msg) => { eventAggregator.GetEvent<MessageBarEnqueuedEvent>().Publish(msg); };
                PastPaperHelperUpdateService.UpdateTaskCompleteEvent += (msg) => { eventAggregator.GetEvent<MessageBarEnqueuedEvent>().Publish(msg); };
                PastPaperHelperUpdateService.UpdateFinalizedEvent += delegate
                {
                    eventAggregator.GetEvent<MessageBarEnqueuedEvent>().Publish($"Updated from {PastPaperHelperCore.CurrentSource.Name}.");
                    Application.Current.Resources["IsLoading"] = Visibility.Hidden;
                    //SettingsViewModel.RefreshSubjectLists();
                    //SettingsViewModel.RefreshSubscription();
                };
                PastPaperHelperUpdateService.UpdateAll(Properties.Settings.Default.SubjectsSubcription);
            }
            else
            {
                //Refresh view models
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
            _regionManager.RequestNavigate("ContentRegion", HamburgerMenu.SelectedItem.ToString().Replace(" ", ""));
        }

        private void Reference_Click(object sender, RoutedEventArgs e)
        {
            HamburgerMenu.SelectedItem = null;
            MenuToggleButton.IsChecked = false;
        }
    }
}
