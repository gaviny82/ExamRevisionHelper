using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper
{
    class MainWindowViewModel : ObservableObject
    {
        public HamburgerMenuItem[] ListItems { get; }

        public MainWindowViewModel()
        {
            ListItems = new HamburgerMenuItem[]
            {
                new HamburgerMenuItem("Search", new SearchView()),
                //new HamburgerMenuItem("Subjects", new SubjectManager()),
                new HamburgerMenuItem("Settings", new SettingsView()),
                new HamburgerMenuItem("About", new AboutView()),
            };
        }
    }
}
