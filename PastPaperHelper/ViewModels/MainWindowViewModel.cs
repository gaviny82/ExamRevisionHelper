using PastPaperHelper.Views;

namespace PastPaperHelper.ViewModels
{
    class MainWindowViewModel : NotificationObject
    {
        public HamburgerMenuItemViewModel[] ListItems { get; }

        public MainWindowViewModel()
        {
            ListItems = new HamburgerMenuItemViewModel[]
            {
                new HamburgerMenuItemViewModel("Search", new SearchView()),
                new HamburgerMenuItemViewModel("Files", new FilesView()),
                new HamburgerMenuItemViewModel("Settings", new SettingsView()),
            };
        }
    }
}
