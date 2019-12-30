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
                Application.Current.Resources["IsLoading"] = Visibility.Visible;
                PastPaperHelperUpdateService.UpdateInitiatedEvent += delegate { mainSnackbar.MessageQueue.Enqueue($"Fetching data from {PastPaperHelperCore.CurrentSource.Name}."); };
                PastPaperHelperUpdateService.UpdateErrorEvent += UpdateMessageCallback;
                PastPaperHelperUpdateService.UpdateTaskCompleteEvent += UpdateMessageCallback;
                PastPaperHelperUpdateService.UpdateFinalizedEvent += delegate { mainSnackbar.MessageQueue.Enqueue($"Updated from {PastPaperHelperCore.CurrentSource.Name}."); };
                PastPaperHelperUpdateService.UpdateAll(Properties.Settings.Default.SubjectsSubcripted);
            }

            //OOBE Test
            //Properties.Settings.Default.FirstRun = true;
            //Properties.Settings.Default.Save();
        }
        public void UpdateMessageCallback(string msg)
        {
            Task.Factory.StartNew(() => MainSnackbar.MessageQueue.Enqueue(msg),
                new CancellationTokenSource().Token,
                TaskCreationOptions.None, SyncContextTaskScheduler);
            //SettingsViewModel.RefreshSubjectLists();
            //SettingsViewModel.RefreshSubscription();
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
