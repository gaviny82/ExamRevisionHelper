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

            if (updateSubjectList) MainSnackbar.MessageQueue.Enqueue("Subject list updated from " + PaperSource.CurrentPaperSource.Name);
            if (updateSubscription) MainSnackbar.MessageQueue.Enqueue("Subscribed subjects updated from " + PaperSource.CurrentPaperSource.Name);
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
    }
}
