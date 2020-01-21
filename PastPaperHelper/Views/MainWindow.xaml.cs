using MaterialDesignThemes.Wpf;
using PastPaperHelper.Models;
using PastPaperHelper.Sources;
using PastPaperHelper.ViewModels;
using System.Linq;
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
        public static DownloadFlyoutViewModel DownloadFlyoutViewModel;

        public MainWindow()
        {
            InitializeComponent();
            MainSnackbar = mainSnackbar;
            Init();

            //OOBE Test
            //Properties.Settings.Default.FirstRun = true;
            //Properties.Settings.Default.Save();
        }
        public async void Init()
        {
            bool updateSubjectList = false, updateSubscription = false;
            await Task.Run(() => SubscriptionManager.CheckUpdate(out updateSubjectList, out updateSubscription));
            if (updateSubjectList || updateSubscription)
            {
                Resources["IsLoading"] = Visibility.Visible;
            }
            await Task.Run(() => SubscriptionManager.UpdateAndInit(updateSubjectList, updateSubscription));

            SettingsViewModel.RefreshSubjectLists();
            SettingsViewModel.RefreshSubscription();
            Resources["IsLoading"] = Visibility.Hidden;
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
