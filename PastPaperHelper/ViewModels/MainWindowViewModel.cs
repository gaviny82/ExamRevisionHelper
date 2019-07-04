﻿using PastPaperHelper.Models;
using System;
using System.Collections.ObjectModel;

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
                new HamburgerMenuItem("Download", new DownloadView()),
                new HamburgerMenuItem("Settings", new SettingsView()),
            };
        }
    }
}
