using MaterialDesignThemes.Wpf;
using PastPaperHelper.Core.Tools;
using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using PastPaperHelper.ViewModels;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace PastPaperHelper.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Snackbar MainSnackbar;
        public static readonly TaskScheduler SyncContextTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        public MainWindow()
        {
            InitializeComponent();
            MainSnackbar = mainSnackbar;

            InitializationResult initResult = (Application.Current as App).InitResult;
            if (initResult == InitializationResult.SuccessUpdateNeeded)
            {
                //TODO: Test needed
                mainSnackbar.MessageQueue.Enqueue($"Last update: {PastPaperHelperCore.LastUpdated.ToLongDateString()}", "Update", () =>
                {
                    Application.Current.Resources["IsLoading"] = Visibility.Visible;
                    PastPaperHelperUpdateService.UpdateAll(Properties.Settings.Default.SubjectsSubcripted);
                });
            }
            else if (initResult == InitializationResult.Error)
            {
                //TODO: Test needed
                PastPaperHelperUpdateService.UpdateInitiatedEvent += delegate 
                { 
                    SnackBar_EnqueueMessage($"Fetching data from {PastPaperHelperCore.CurrentSource.Name}...");
                    Application.Current.Resources["IsLoading"] = Visibility.Visible;
                };
                PastPaperHelperUpdateService.UpdateErrorEvent += (msg) => { SnackBar_EnqueueMessage(msg); };
                PastPaperHelperUpdateService.UpdateTaskCompleteEvent += (msg) => { SnackBar_EnqueueMessage(msg); };
                PastPaperHelperUpdateService.UpdateFinalizedEvent += delegate 
                { 
                    SnackBar_EnqueueMessage($"Updated from {PastPaperHelperCore.CurrentSource.Name}.");
                    Application.Current.Resources["IsLoading"] = Visibility.Hidden;
                    //SettingsViewModel.RefreshSubjectLists();
                    //SettingsViewModel.RefreshSubscription();
                };
                PastPaperHelperUpdateService.UpdateAll(Properties.Settings.Default.SubjectsSubcripted);
            }
        }
        public static void SnackBar_EnqueueMessage(string msg)
        {
            Task.Factory.StartNew(() => MainSnackbar.MessageQueue.Enqueue(msg),
                new CancellationTokenSource().Token,
                TaskCreationOptions.None, SyncContextTaskScheduler);
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
            double newWidth = e.NewSize.Width, newHeight = e.NewSize.Height;
            dlgGrid.Width = newWidth - 350;
            dlgGrid.Height = newHeight - 150;
        }

        private void AddSubject_Click(object sender, RoutedEventArgs e)
        {
            if (!(selectionTreeView.SelectedItem is Subject)) return;
            Subject item = (Subject)selectionTreeView.SelectedItem;
            SettingsViewModel vm = ((DataContext as MainWindowViewModel).ListItems.Last().Content as SettingsView).DataContext as SettingsViewModel;
            vm.AddSubjectCommand.Execute(item);
        }

    }
}
