using MaterialDesignThemes.Wpf;
using PastPaperHelper.PrismTest.ViewModels;
using Prism.Regions;
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
        public static Snackbar MainSnackbar;

        public static TaskScheduler SyncContextTaskScheduler { get; internal set; }

        private readonly IRegionManager _regionManager;
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            MainSnackbar = mainSnackbar;
            SyncContextTaskScheduler = TaskScheduler.Current;
            _regionManager = regionManager;
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
            string uri = HamburgerMenu.SelectedItem?.ToString().Replace(" ", "");
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
