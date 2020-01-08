using MaterialDesignThemes.Wpf;
using PastPaperHelper.PrismTest.ViewModels;
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

        public MainWindow()
        {
            InitializeComponent();
            MainSnackbar = mainSnackbar;
            SyncContextTaskScheduler = TaskScheduler.Current;
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
            //dlgGrid.Width = newWidth - 350;
            //dlgGrid.Height = newHeight - 150;
        }

        private void AddSubject_Click(object sender, RoutedEventArgs e)
        {
            //if (!(selectionTreeView.SelectedItem is Subject)) return;
            //Subject item = (Subject)selectionTreeView.SelectedItem;
            //SettingsViewModel vm = ((DataContext as MainWindowViewModel).ListItems.Last().Content as SettingsView).DataContext as SettingsViewModel;
            //vm.AddSubjectCommand.Execute(item);
        }

        private void HamburgerMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as MainWindowViewModel).NavigateCommand.Execute(HamburgerMenu.SelectedItem.ToString().Replace(" ", ""));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel).NavigateCommand.Execute(HamburgerMenu.SelectedItem.ToString().Replace(" ", ""));
        }
    }
}
