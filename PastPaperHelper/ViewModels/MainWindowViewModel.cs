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
                new HamburgerMenuItemViewModel("Papers", new FilesView()),
                new HamburgerMenuItemViewModel("Files", new LocalStorage()),
                new HamburgerMenuItemViewModel("Search", new SearchView()),
                new HamburgerMenuItemViewModel("Settings", new SettingsView()),
            };
        }
    }
}
