// ------------------------------------------------------
// ---------- Copyright (c) 2017 Colton Murphy ----------
// ------------------------------------------------------
// ------------------------------------------------------
// ------------------------------------------------------
// ------------------------------------------------------
// ------------------------------------------------------
// ------------------------------------------------------
// ------------------------------------------------------
// ------------------------------------------------------
// ------------------------------------------------------
// ------------------------------------------------------
// ------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using Common.Wpf.Data;

namespace Test
{
    public class TestProgram
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Create the app
            Application app = new Application();

            // Create the main window
            app.MainWindow = new MainWindow();

            // Show the main window
            app.MainWindow.Show();

            // Run the app
            app.Run();
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the item has children, then show the checkbox, otherwise hide it
            return ((bool)value ? Visibility.Visible : Visibility.Hidden);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LevelConverter : IValueConverter
    {
        public GridLength LevelWidth { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Return the width multiplied by the level
            return ((int)value * LevelWidth.Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Item : TreeGridElement
    {
        public int Value { get; private set; }
        public string Name { get; private set; }

        public Item(string name, int value, bool hasChildren)
        {
            // Initialize the item
            Name = name;
            Value = value;
            HasChildren = hasChildren;
        }
    }

}