namespace PastPaperHelper
{
    class MainWindowViewModel : NotificationObject
    {
        public HamburgerMenuItem[] ListItems { get; }

        public MainWindowViewModel()
        {
            ListItems = new HamburgerMenuItem[]
            {
                new HamburgerMenuItem("Search", new SearchView()),
                new HamburgerMenuItem("Download", new PaperDownloadView()),
                new HamburgerMenuItem("Settings", new SettingsView()),
            };
        }
    }
}
