using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ExamRevisionHelper.Core;
using ExamRevisionHelper.ViewModels;
using MaterialDesignThemes.Wpf;

namespace ExamRevisionHelper.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Snackbar MainSnackbar { get; internal set; }//TODO: Remove in the future
        public static TaskScheduler SyncContextTaskScheduler { get; internal set; }//TODO: Remove in the future

        public MainWindow()
        {
            SyncContextTaskScheduler = TaskScheduler.Current;
            InitializeComponent();
            MainSnackbar = mainSnackbar;

            var subsColl = Properties.Settings.Default.SubjectsSubcription;
            string[] subscribedSubjects = new string[subsColl.Count];
            subsColl.CopyTo(subscribedSubjects, 0);

            ExamRevisionHelperUpdater.UpdateServiceNotifiedEvent += (args) =>
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
                        MainWindowViewModel.RefreshSubscribedSubjects();
                        SubjectDialogViewModel.RefreshSubjectLists();
                    }
                    mainSnackbar.MessageQueue.Enqueue(args.Message);
                });
            };
            ExamRevisionHelperUpdater.UpdateServiceErrorEvent += (args) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.Resources["IsLoading"] = Visibility.Hidden;
                    if (args.ErrorType == ErrorType.SubjectNotSupported)
                    {
                        if (args.Exception is SubjectUnsupportedException exception)
                        {
                            string[] unsupportedList = exception.UnsupportedSubjects;
                            foreach (string item in unsupportedList)
                            {
                                Properties.Settings.Default.SubjectsSubcription.Remove(item);
                            }
                            Properties.Settings.Default.Save();
                            mainSnackbar.MessageQueue.Enqueue($"{args.ErrorMessage}, automatically removed from subscription. Go to Settings page to check details.", "SETTINGS", () => { HamburgerMenu.SelectedIndex = HamburgerMenu.Items.Count - 1; }, true);
                        }
                    }
                    else mainSnackbar.MessageQueue.Enqueue(args.ErrorMessage, "RETRY", () => { ExamRevisionHelperUpdater.UpdateAll(subscribedSubjects); });
                });
            };

            ExamRevisionHelperUpdater.SubjectSubscribedEvent += (subj) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    mainSnackbar.MessageQueue.Enqueue(
                    content: $"{subj.SyllabusCode} {subj.Curriculum} {subj.Name} is added to your subscription.",
                    actionContent: null,
                    actionHandler: null, null,
                    promote: false,
                    neverConsiderToBeDuplicate: false,
                    durationOverride: TimeSpan.FromSeconds(1));

                    MainWindowViewModel.SubscribedSubjects.Add(subj);
                    if (!Properties.Settings.Default.SubjectsSubcription.Contains(subj.SyllabusCode))
                    {
                        Properties.Settings.Default.SubjectsSubcription.Add(subj.SyllabusCode);
                        Properties.Settings.Default.Save();
                    }
                });
            };

            InitializationResult initResult = (Application.Current as App).InitResult;
            if (initResult == InitializationResult.SuccessUpdateNeeded)
            {
                mainSnackbar.MessageQueue.Enqueue(
                    content: $"Update needed. (Last updated: {(Application.Current as App).CoreInstance.CurrentSource.LastUpdated.ToShortDateString()})",
                    actionContent: "UPDATE",
                    actionHandler: (param) => { ExamRevisionHelperUpdater.UpdateAll(subscribedSubjects); }, null,
                    promote: true,
                    neverConsiderToBeDuplicate: true,
                    durationOverride: TimeSpan.FromSeconds(5));
            }
            else if (initResult == InitializationResult.Error)
            {
                mainSnackbar.MessageQueue.Enqueue(
                    content: $"An error has occurred. Try reloading from {(Application.Current as App).CoreInstance.CurrentSource.DisplayName}",
                    actionContent: "RELOAD",
                    actionHandler: (param) => { ExamRevisionHelperUpdater.UpdateAll(subscribedSubjects); }, null,
                    promote: true,
                    neverConsiderToBeDuplicate: true,
                    durationOverride: TimeSpan.FromDays(1));
                return;
            }
            MainWindowViewModel.RefreshSubscribedSubjects();
            SubjectDialogViewModel.RefreshSubjectLists();
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
