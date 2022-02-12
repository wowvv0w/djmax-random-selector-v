using Caliburn.Micro;
using DjmaxRandomSelectorV.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class InfoViewModel : Screen
    {
        private const string GITHUB_PAGE_URL = "https://github.com/wowvv0w/djmax-random-selector-v";
        private const string BUG_REPORT_URL = "https://github.com/wowvv0w/djmax-random-selector-v/issues";

        private DockPanel _dockPanel;

        public InfoViewModel(int currentVersion, int lastestVersion, DockPanel dockPanel)
        {
            CurrentVersion = IntToString(currentVersion);
            LastestVersion = IntToString(lastestVersion);
            AllTrackVersion = Settings.Default.allTrackVersion.ToString();

            _dockPanel = dockPanel;

            string IntToString(int version)
            {
                var str = version.ToString();
                for (int i = 1; i <= 3; i += 2)
                {
                    str = str.Insert(i, ".");
                }
                return str;
            }
        }

        private string _currentVersion;
        private string _lastestVersion;
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
        public string LastestVersion
        {
            get { return _lastestVersion; }
            set
            {
                _lastestVersion = $"Lastest Version: {value}";
                NotifyOfPropertyChange(() => LastestVersion);
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
            _dockPanel.Effect = null;
        }
    }
}
