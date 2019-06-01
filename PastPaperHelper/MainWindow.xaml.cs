using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using Spire.Pdf;
using Spire.Pdf.General.Find;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace PastPaperHelper
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Snackbar MainSnackbar;
        public MainWindow()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTA0NzY1QDMxMzcyZTMxMmUzMEJ2UkIvNUk0T0M2OXNEYnY0cFRWUnNWUWZ0QkFyR3NVaDBlbjZuYi9NUEU9;MTA0NzY2QDMxMzcyZTMxMmUzMEVsMFlPRElYWjgyNUFYYjZBVXN2R2RHRW05QlYzYVd6S0NUL093R29PcEk9;MTA0NzY3QDMxMzcyZTMxMmUzMExYTEpvcCtUNWJ2T3NaQ01aZGJGbloxamFSN2lOaGlRM0UwUmlQcFBLSzg9");
            InitializeComponent();
            MainSnackbar = mainSnackbar;
            mainSnackbar.MessageQueue.Enqueue("Hello world!");
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
