using PastPaperHelper.Views;
using System.Collections.Generic;

namespace PastPaperHelper.ViewModels
{
    class MainWindowViewModel : NotificationObject
    {
        public static List<string> Files = new List<string>();
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
