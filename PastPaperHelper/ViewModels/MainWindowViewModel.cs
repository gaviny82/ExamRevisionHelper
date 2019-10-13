using PastPaperHelper.Views;
using System.Windows.Controls;

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
                new HamburgerMenuItemViewModel("Download", new Grid()),
                new HamburgerMenuItemViewModel("Local Storage", new LocalStorage()),
                new HamburgerMenuItemViewModel("Search", new SearchView()),
                new HamburgerMenuItemViewModel("Settings", new SettingsView()),
            };
        }
    }
}
