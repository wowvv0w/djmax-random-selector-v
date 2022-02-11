﻿using Caliburn.Micro;
using DjmaxRandomSelectorV.Properties;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class InfoViewModel : Screen
    {
        private const string GITHUB_PAGE_URL = "https://github.com/wowvv0w/djmax-random-selector-v";
        private const string BUG_REPORT_URL = "https://github.com/wowvv0w/djmax-random-selector-v/issues";

        public InfoViewModel(int currentVersion, int lastVersion)
        {
            CurrentVersion = IntToString(currentVersion);
            LastVersion = IntToString(lastVersion);
            AllTrackVersion = Settings.Default.allTrackVersion.ToString();

            string IntToString(int version)
            {
                var str = version.ToString();
                for (int i = 1; i <= 5; i += 2)
                {
                    str = str.Insert(i, ".");
                }
                return str;
            }
        }

        private string _currentVersion;
        private string _lastVersion;
        private string _allTrackVersion;
        public string CurrentVersion
        {
            get { return _currentVersion; }
            set
            {
                _currentVersion = $"Current Version: {value}";
                NotifyOfPropertyChange(() => CurrentVersion);
            }
        }
        public string LastVersion
        {
            get { return _lastVersion; }
            set
            {
                _lastVersion = $"Last Version: {value}";
                NotifyOfPropertyChange(() => LastVersion);
            }
        }
        public string AllTrackVersion
        {
            get { return _allTrackVersion; }
            set
            {
                _allTrackVersion = $"All Track Version: {value}";
                NotifyOfPropertyChange(() => AllTrackVersion);
            }
        }

        public void OpenGithubPage()
        {
            Process.Start(GITHUB_PAGE_URL);
        }

        public void OpenBugReport()
        {
            Process.Start(BUG_REPORT_URL);
        }

        public void Close()
        {
            TryCloseAsync();
        }

        public void MoveWindow(object view)
        {
            var window = view as Window;
            window.DragMove();
        }
    }
}
