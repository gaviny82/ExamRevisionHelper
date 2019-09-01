using PastPaperHelper.Views;
using System.Windows;

namespace PastPaperHelper.ViewModels
{
    class MainWindowViewModel : NotificationObject
    {
        public HamburgerMenuItemViewModel[] ListItems { get; }

        public MainWindowViewModel()
        {
            ListItems = new HamburgerMenuItemViewModel[]
            {
                new HamburgerMenuItemViewModel("Files", new FilesView()),
                new HamburgerMenuItemViewModel("Search", new SearchView()),
                new HamburgerMenuItemViewModel("Settings", new SettingsView()),
            };
        }
    }
}
