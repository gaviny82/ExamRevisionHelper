using MaterialDesignThemes.Wpf;
using PastPaperHelper.Sources;
using PastPaperHelper.ViewModels;
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

        public MainWindow()
        {
            InitializeComponent();
            MainSnackbar = mainSnackbar;
            
            //OOBE Test
            //Properties.Settings.Default.FirstRun = true;
            //Properties.Settings.Default.Save();

            bool updateSubjectList =false, updateSubscription=false;
            Task.Factory.StartNew(() =>
            {
                SubscriptionManager.CheckUpdate(out updateSubjectList, out updateSubscription);
                SubscriptionManager.UpdateAndInit(updateSubjectList, updateSubscription);
            }).ContinueWith(t =>
            {
                if (updateSubjectList) MainSnackbar.MessageQueue.Enqueue("Subject list updated from " + PaperSource.CurrentPaperSource.Name);
                if (updateSubscription) MainSnackbar.MessageQueue.Enqueue("Subscribed subjects updated from " + PaperSource.CurrentPaperSource.Name);
                SettingsViewModel.RefreshSubjectLists();
                SettingsViewModel.RefreshSubscription();
                //((DataContext as MainWindowViewModel).ListItems[1].Content as FilesView).UpdateSelectedItem();
            }, TaskScheduler.FromCurrentSynchronizationContext());

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
    }
}
